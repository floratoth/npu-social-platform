using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;

[Route("api/npu")]
[ApiController]
public class NpuController : ControllerBase
{
    private List<Npu> npus;

    public NpuController()
    {
        Console.WriteLine("cica");
        using (StreamReader r = new StreamReader("MOCK_DATA.json"))
        {
            string json = r.ReadToEnd();
            npus = JsonSerializer.Deserialize<List<Npu>>(json);
        }
    }

    // GET api/npu
    [HttpGet]
    public ActionResult<List<Npu>> GetAllNpus()
    {
         var json = JsonSerializer.Serialize(npus);
    Console.WriteLine("npu-s: " + json);
        return npus;
    }

// GET api/npu/{id}
[HttpGet("{id}")]
public ActionResult<Npu> GetNpuById(string id)
{
    var npu = npus.FirstOrDefault(n => n.Id == id);
    if (npu == null)
    {
        return NotFound();
    }

    return npu;
}

[HttpPut("{id}")]
public ActionResult<Npu> UpdateNpu(string id, [FromBody] Npu updatedNpu)
{
    var npu = npus.FirstOrDefault(n => n.Id == id);
    if (npu == null)
    {
        return NotFound();
    }

    npu.Creativity.ScoreValue = npu.Creativity.ScoreValue * npu.Creativity.Votes + updatedNpu.Creativity.ScoreValue;
    npu.Creativity.Votes++;
    npu.Creativity.ScoreValue /= npu.Creativity.Votes;
    
    npu.Uniqueness.ScoreValue = npu.Uniqueness.ScoreValue * npu.Uniqueness.Votes + updatedNpu.Uniqueness.ScoreValue;
    npu.Uniqueness.Votes++;
    npu.Uniqueness.ScoreValue /= npu.Uniqueness.Votes;

    var npuIndex = npus.FindIndex(n => n.Id == id);
    if (npuIndex != -1)
    {
        npus[npuIndex] = npu;
    }

    // Save the updated list of npus back to the JSON file
    string json = JsonSerializer.Serialize(npus);
    System.IO.File.WriteAllText("MOCK_DATA.json", json);

    return npu;  // Return the updated npu
}
}

