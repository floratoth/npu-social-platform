using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

[Route("api/npu")]
[ApiController]
public class NpuController : ControllerBase
{
    private readonly AmazonDynamoDBClient client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
{
    RegionEndpoint = RegionEndpoint.EUNorth1
});
    private static string tableName = "npu";

[HttpPost]
public async Task<ActionResult<NpuDto>> CreateNpu([FromBody] NpuDto newNpu)
{
    Table npuTable = Table.LoadTable(client, tableName);

    // Generate an ID for the new Npu
    newNpu.Id = Guid.NewGuid().ToString();

    // Initialize the scores
    newNpu.Creativity = new ScoreDto { Score = 0, Votes = 0 };
    newNpu.Uniqueness = new ScoreDto { Score = 0, Votes = 0 };

    var npuDocument = new Document();
    npuDocument["Id"] = newNpu.Id;
    npuDocument["Creativity"] = new Document
    {
        ["Score"] = newNpu.Creativity.Score,
        ["Votes"] = newNpu.Creativity.Votes
    };
    npuDocument["Uniqueness"] = new Document
    {
        ["Score"] = newNpu.Uniqueness.Score,
        ["Votes"] = newNpu.Uniqueness.Votes
    };

    npuDocument["Description"] = newNpu.Description;
    npuDocument["Name"] = newNpu.Name;
    npuDocument["ImageUrl"] = newNpu.ImageUrl;
    await npuTable.PutItemAsync(npuDocument);

    // Fetch the saved item from DynamoDB
    GetItemOperationConfig config = new GetItemOperationConfig
    {
        AttributesToGet = new List<string> { "Id", "Creativity", "Uniqueness", "Description", "Name", "ImageUrl" },
        ConsistentRead = true
    };
    Document savedNpu = await npuTable.GetItemAsync(newNpu.Id, config);

    // Map the saved item to NpuDto
    var npuDto = new NpuDto
{
    Id = savedNpu["Id"].AsString(),
    Creativity = new ScoreDto
    {
        Score = savedNpu["Creativity"].AsDocument()["Score"].AsInt(),
        Votes = savedNpu["Creativity"].AsDocument()["Votes"].AsInt()
    },
    Uniqueness = new ScoreDto
    {
        Score = savedNpu["Uniqueness"].AsDocument()["Score"].AsInt(),
        Votes = savedNpu["Uniqueness"].AsDocument()["Votes"].AsInt()
    },
    Description = savedNpu["Description"].AsString(),
    Name = savedNpu["Name"].AsString(),
    ImageUrl = savedNpu["ImageUrl"].AsString()
};


    return npuDto;
}

// This method handles GET requests to /api/npu and returns all Npu
[HttpGet]
public async Task<ActionResult<IEnumerable<NpuDto>>> GetAllNpus()
{
    // Load the DynamoDB table
    Table npuTable = Table.LoadTable(client, tableName);

    // Scan the DynamoDB table
    ScanFilter scanFilter = new ScanFilter();
    Search search = npuTable.Scan(scanFilter);

    List<Document> npuDocuments = new List<Document>();
    do
    {
        npuDocuments.AddRange(await search.GetNextSetAsync());
    }
    while (!search.IsDone);

    // Map the DynamoDB Documents to NpuDto objects
var npusDto = npuDocuments.Select(npuDocument => new NpuDto
{
    Id = npuDocument["Id"].AsString(),
    Creativity = new ScoreDto
    {
        Score = npuDocument["Creativity"].AsDocument()["Score"].AsDouble(),
        Votes = npuDocument["Creativity"].AsDocument()["Votes"].AsInt()
    },
    Uniqueness = new ScoreDto
    {
        Score = npuDocument["Uniqueness"].AsDocument()["Score"].AsDouble(),
        Votes = npuDocument["Uniqueness"].AsDocument()["Votes"].AsInt()
    },
    Description = npuDocument["Description"].AsString(),
    Name = npuDocument["Name"].AsString(),
    ImageUrl = npuDocument["ImageUrl"].AsString()
}).ToList();

    // Return the list of NpuDto objects
    return npusDto;
}


// This method handles GET requests to /api/npu/{id}
[HttpGet("{id}")]
public async Task<ActionResult<NpuDto>> GetNpuById(string id)
{
    // Load the DynamoDB table
    Table npuTable = Table.LoadTable(client, tableName);
    
    // Configure the read operation to retrieve specific attributes
    GetItemOperationConfig config = new GetItemOperationConfig
    {
        AttributesToGet = new List<string> { "Id", "Creativity", "Uniqueness", "Description", "Name", "ImageUrl" },
        ConsistentRead = true
    };

    // Get the item from DynamoDB by Id
    Document npuDocument = await npuTable.GetItemAsync(id, config);

    // If the item does not exist, return a 404 Not Found status
    if (npuDocument == null)
    {
        return NotFound();
    }

    // Create a new NpuDto object and map the DynamoDB item to it
    var npuDto = new NpuDto
    {
        Id = npuDocument["Id"].AsString(),
        Creativity = new ScoreDto
        {
            Score = npuDocument["Creativity"].AsDocument()["Score"].AsInt(),
            Votes = npuDocument["Creativity"].AsDocument()["Votes"].AsInt()
        },
        Uniqueness = new ScoreDto
        {
            Score = npuDocument["Uniqueness"].AsDocument()["Score"].AsInt(),
            Votes = npuDocument["Uniqueness"].AsDocument()["Votes"].AsInt()
        },
        Description = npuDocument["Description"].AsString(),
        Name = npuDocument["Name"].AsString(),
        ImageUrl = npuDocument["ImageUrl"].AsString()
    };

    // Return the NpuDto object
    return npuDto;
}

[HttpPut("{id}")]
public async Task<ActionResult<NpuDto>> UpdateNpu(string id, [FromBody] NpuDto updatedNpu)
{
    // Load the DynamoDB table
    Table npuTable = Table.LoadTable(client, tableName);

    // Configure the read operation to retrieve specific attributes
    GetItemOperationConfig config = new GetItemOperationConfig
    {
        AttributesToGet = new List<string> { "Id", "Creativity", "Uniqueness", "Description", "Name", "ImageUrl" },
        ConsistentRead = true
    };

    // Get the item from DynamoDB by Id
    Document npuDocument = await npuTable.GetItemAsync(id, config);

    // If the item does not exist, return a 404 Not Found status
    if (npuDocument == null)
    {
        return NotFound();
    }

    // Update the scores
    int oldCreativityVotes = npuDocument["Creativity"].AsDocument()["Votes"].AsInt();
    int oldUniquenessVotes = npuDocument["Uniqueness"].AsDocument()["Votes"].AsInt();

    npuDocument["Creativity"] = new Document
{
    ["Score"] = Math.Round((npuDocument["Creativity"].AsDocument()["Score"].AsDouble() * oldCreativityVotes + updatedNpu.Creativity.Score) / (oldCreativityVotes + 1), 2),
    ["Votes"] = oldCreativityVotes + 1
};

npuDocument["Uniqueness"] = new Document
{
    ["Score"] = Math.Round((npuDocument["Uniqueness"].AsDocument()["Score"].AsDouble() * oldUniquenessVotes + updatedNpu.Uniqueness.Score) / (oldUniquenessVotes + 1), 2),
    ["Votes"] = oldUniquenessVotes + 1
};



    // Update the item in DynamoDB
    await npuTable.UpdateItemAsync(npuDocument);

    // Map the updated item to NpuDto
    var npuDto = new NpuDto
    {
        Id = npuDocument["Id"].AsString(),
        Creativity = new ScoreDto
{
    Score = npuDocument["Creativity"].AsDocument()["Score"].AsDouble(),
    Votes = npuDocument["Creativity"].AsDocument()["Votes"].AsInt()
},
Uniqueness = new ScoreDto
{
    Score = npuDocument["Uniqueness"].AsDocument()["Score"].AsDouble(),
    Votes = npuDocument["Uniqueness"].AsDocument()["Votes"].AsInt()
},

        Description = npuDocument["Description"].AsString(),
        Name = npuDocument["Name"].AsString(),
        ImageUrl = npuDocument["ImageUrl"].AsString()
    };

    // Return the updated NpuDto
    return npuDto;
}



// // This method handles PUT requests to /api/npu/{id} and updates an existing Npu
// [HttpPut("{id}")]
// public async Task<ActionResult<NpuDto>> UpdateNpu(string id, [FromBody] NpuDto updatedNpu)
// {
//     // Load the DynamoDB table
//     Table npuTable = Table.LoadTable(client, tableName);

//     // Get the item from DynamoDB
//     Document npuDocument = await npuTable.GetItemAsync(id);

//     if (npuDocument == null)
//     {
//         return NotFound();
//     }

//     // Calculate the new Creativity score
//     double oldCreativityScore = npuDocument["Creativity"].AsDocument()["Score"].AsDouble();
//     int oldCreativityVotes = npuDocument["Creativity"].AsDocument()["Votes"].AsInt();
//     double newCreativityScore = (oldCreativityScore * oldCreativityVotes + updatedNpu.Creativity.Score) / (oldCreativityVotes + 1);

//     // Calculate the new Uniqueness score
//     double oldUniquenessScore = npuDocument["Uniqueness"].AsDocument()["Score"].AsDouble();
//     int oldUniquenessVotes = npuDocument["Uniqueness"].AsDocument()["Votes"].AsInt();
//     double newUniquenessScore = (oldUniquenessScore * oldUniquenessVotes + updatedNpu.Uniqueness.Score) / (oldUniquenessVotes + 1);

// // Define the update expression
// string updateExpression = "SET Creativity.Score = :c, Creativity.Votes = Creativity.Votes + :cv, Uniqueness.Score = :u, Uniqueness.Votes = Uniqueness.Votes + :uv";


// // Define the attribute values
// var attributeValues = new Dictionary<string, AttributeValue>
// {
//     {":c", new AttributeValue { N = newCreativityScore.ToString() }},
//     {":cv", new AttributeValue { N = "1" }},
//     {":u", new AttributeValue { N = newUniquenessScore.ToString() }},
//     {":uv", new AttributeValue { N = "1" }}
// };

//     // Update the item in DynamoDB
//     var updateItem = new UpdateItemRequest
//     {
//         TableName = tableName,
//         Key = new Dictionary<string, AttributeValue>() { { "Id", new AttributeValue { S = id } } },
//         ExpressionAttributeValues = attributeValues,
//         UpdateExpression = updateExpression,
//         ReturnValues = ReturnValue.UPDATED_NEW
//     };

//     UpdateItemResponse updatedResponse = await client.UpdateItemAsync(updateItem);

//     if (updatedResponse.HttpStatusCode == HttpStatusCode.OK)
// {
//     var updatedAttributes = updatedResponse.Attributes;

//     var npuDto = new NpuDto
//     {
//         Id = npuDocument["Id"].AsString(),
//         Creativity = new ScoreDto
//         {
//             Score = double.Parse(updatedAttributes["Creativity"].M["Score"].N),
//             Votes = int.Parse(updatedAttributes["Creativity"].M["Votes"].N)
//         },
//         Uniqueness = new ScoreDto
//         {
//             Score = double.Parse(updatedAttributes["Uniqueness"].M["Score"].N),
//             Votes = int.Parse(updatedAttributes["Uniqueness"].M["Votes"].N)
//         },
//         Description = npuDocument["Description"].AsString(),
//         Name = npuDocument["Name"].AsString(),
//         ImageUrl = npuDocument["ImageUrl"].AsString()
//     };

//     return npuDto;
// }
// else
// {
//     return Problem("An error occurred while updating the NPU.");
// }

// }

}
// public class NpuController : ControllerBase
// {
//     private List<Npu> npus;

//     public NpuController()
//     {
//         using (StreamReader r = new StreamReader("MOCK_DATA.json"))
//         {
//             string json = r.ReadToEnd();
//             npus = JsonSerializer.Deserialize<List<Npu>>(json);
//         }
//     }

//     // GET api/npu
//     [HttpGet]
//     public ActionResult<List<Npu>> GetAllNpus()
//     {
//         var json = JsonSerializer.Serialize(npus);
//         return npus;
//     }

// // GET api/npu/{id}
// [HttpGet("{id}")]
// public ActionResult<Npu> GetNpuById(string id)
// {
//     var npu = npus.FirstOrDefault(n => n.Id == id);
//     if (npu == null)
//     {
//         return NotFound();
//     }

//     return npu;
// }

// [HttpPut("{id}")]
// public ActionResult<Npu> UpdateNpu(string id, [FromBody] Npu updatedNpu)
// {
//     var npu = npus.FirstOrDefault(n => n.Id == id);
//     if (npu == null)
//     {
//         return NotFound();
//     }

//     npu.Creativity.Score = npu.Creativity.Score * npu.Creativity.Votes + updatedNpu.Creativity.Score;
//     npu.Creativity.Votes++;
//     npu.Creativity.Score /= npu.Creativity.Votes;
    
//     npu.Uniqueness.Score = npu.Uniqueness.Score * npu.Uniqueness.Votes + updatedNpu.Uniqueness.Score;
//     npu.Uniqueness.Votes++;
//     npu.Uniqueness.Score /= npu.Uniqueness.Votes;

//     var npuIndex = npus.FindIndex(n => n.Id == id);
//     if (npuIndex != -1)
//     {
//         npus[npuIndex] = npu;
//     }

//     // Save the updated list of npus back to the JSON file
//     string json = JsonSerializer.Serialize(npus);
//     System.IO.File.WriteAllText("MOCK_DATA.json", json);

//     return npu;  // Return the updated npu
// }

// [HttpPost]
// public ActionResult<Npu> CreateNpu([FromBody] Npu newNpu)
// {
//     // Generate an ID for the new Npu
//     newNpu.Id = Guid.NewGuid().ToString();

//     // Initialize the scores
//     newNpu.Creativity = new Score { Score = 0, Votes = 0 };
//     newNpu.Uniqueness = new Score { Score = 0, Votes = 0 };

//     // Add the new Npu to the list
//     npus.Add(newNpu);

//     // Save the updated list of npus back to the JSON file
//     string json = JsonSerializer.Serialize(npus);
//     System.IO.File.WriteAllText("MOCK_DATA.json", json);

//     return newNpu;  // Return the created npu
// }
// }

