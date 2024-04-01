"use client";
import { usePathname, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import npuData from "@/MOCK_DATA.json";
import { Npu } from "@/models/NpuModel";
import { ChevronLeftIcon } from "@heroicons/react/20/solid";
import { LightBulbIcon, FingerPrintIcon } from "@heroicons/react/20/solid";

interface NpuDetailProps {
  npuData: Npu;
}

export default function NpuDetail({ npuData }: NpuDetailProps) {
  const router = useRouter();
  const pathname = usePathname();
  const id = pathname.split("/").pop();

  const [npu, setNpu] = useState<Npu | null>(npuData);
  const [creativityScore, setCreativityScore] = useState(0);
  const [uniquenessScore, setUniquenessScore] = useState(0);
  const [hasVoted, setHasVoted] = useState(false);

  // Fetch the NPU data when the component mounts
  useEffect(() => {
    if (id) {
      fetch(`http://localhost:5105/api/npu/${id}`)
        .then((response) => response.json())
        .then((data) => setNpu(data))
        .catch((error) => console.error(error));
    }
  }, [id]);

  const updateScores = () => {
    if (!npu) {
      return;
    }

    const updatedNpu = {
      ...npu,
      creativity: {
        ...npu.creativity,
        score: creativityScore,
      },
      uniqueness: {
        ...npu.uniqueness,
        score: uniquenessScore,
      },
    };

    fetch(`http://localhost:5105/api/npu/${npu.id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(updatedNpu),
    })
      .then((response) => response.json())
      .then((updatedNpu) => {
        console.log("UPDATED NPU: ", updatedNpu);
        setHasVoted(true);

        // Hide the message after 5 seconds
        setTimeout(() => {
          setHasVoted(false);
        }, 5000); // 5000ms = 5 seconds
        setNpu(updatedNpu);
        setCreativityScore(0);
        setUniquenessScore(0);
      })
      .catch((err) => {
        console.error(err);
        setHasVoted(false);
      });
  };

  if (!npu) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <button
        onClick={() => router.back()}
        className="mt-4 px-2 md:px-8 py-2 text-white bg-blue-500 border border-blue-500 rounded ml-4 md:ml-8 flex items-center"
      >
        <ChevronLeftIcon className="h-5 w-5 mr-2" />
        Back
      </button>
      <div className="max-w-7xl mx-auto w-full md:w-auto flex flex-col md:flex-row p-4 bg-gray-50 rounded-lg shadow-s border border-gray-200 my-4">
        <div className="w-full md:w-1/2 mb-4 md:mb-0">
          <img
            className="object-cover"
            style={{ width: "500px", height: "700px" }}
            src={npu.imageUrl}
            alt={npu.name}
          />
        </div>
        <div className="w-full md:w-1/2 px-4 py-4">
          <h1 className="text-4xl font-bold mb-4">{npu.name}</h1>
          <p
            className="text-lg mb-4"
            style={{ width: "100%", whiteSpace: "pre-wrap" }}
          >
            {npu.description}
          </p>
          <div className="space-y-2">
            <div className="flex items-center">
              <LightBulbIcon className="h-7 w-7 text-yellow-500 mr-2" />
              <strong className="font-bold">Creativity:</strong>{" "}
              <span className="text-sm font-semibold text-gray-700 ml-2">
                {npu.creativity.score ? npu.creativity.score.toFixed(2) : 0}
              </span>
            </div>
            <div className="flex items-center">
              <FingerPrintIcon className="h-7 w-7 text-yellow-500 mr-2" />
              <strong className="font-bold">Uniqueness:</strong>{" "}
              <span className="text-sm font-semibold text-gray-700 ml-2">
                {npu.uniqueness.score ? npu.uniqueness.score.toFixed(2) : 0}
              </span>
            </div>
          </div>
          <div className="mt-4 p-4 border border-gray-200 rounded shadow-sm">
            <h2 className="text-2xl font-bold mb-4 text-center">
              Cast Your Votes for this NPU!
            </h2>
            <div className="flex flex-col md:flex-row gap-4 mt-6">
              <div>
                <label
                  htmlFor="creativityScore"
                  className="block text-sm font-medium text-gray-700"
                >
                  Creativity Score: {creativityScore}
                </label>
                <input
                  type="range"
                  id="creativityScore"
                  name="creativityScore"
                  min="1"
                  max="10"
                  value={creativityScore}
                  onChange={(e) => setCreativityScore(Number(e.target.value))}
                  className="mt-1 block w-full cursor-pointer"
                />
              </div>
              <div>
                <label
                  htmlFor="uniquenessScore"
                  className="block text-sm font-medium text-gray-700"
                >
                  Uniqueness Score: {uniquenessScore}
                </label>
                <input
                  type="range"
                  id="uniquenessScore"
                  name="uniquenessScore"
                  min="1"
                  max="10"
                  value={uniquenessScore}
                  onChange={(e) => setUniquenessScore(Number(e.target.value))}
                  className="mt-1 block w-full cursor-pointer"
                />
              </div>
              <button
                onClick={updateScores}
                disabled={hasVoted}
                className="self-start md:self-auto px-6 py-3 border border-transparent text-sm font-medium rounded-md text-white bg-green-500 hover:bg-green-600 mt-4 md:mt-0 ml-auto"
              >
                I vote!
              </button>
            </div>
            {hasVoted && (
              <div className="mt-4 p-4 border border-gray-200 rounded shadow-sm bg-green-50 text-green-700">
                <h2 className="text-2xl font-bold mb-4 text-center">
                  Thank You for Voting!
                </h2>
                <p className="text-center">
                  Your votes for Creativity and Uniqueness have been recorded.
                  We appreciate your contribution!
                </p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
