"use client";
import { useState, useEffect } from "react";
import Link from "next/link";
import {
  ChevronRightIcon,
  LightBulbIcon,
  FingerPrintIcon,
} from "@heroicons/react/20/solid";
import { Npu } from "@/models/NpuModel";

export default function Home() {
  const [npuData, setNpuData] = useState<Npu[]>([]);

  useEffect(() => {
    const fetchNpus = async () => {
      const response = await fetch("http://localhost:5105/api/npu");
      if (!response.ok) {
        console.error("Failed to fetch npus");
        return;
      }

      const npus = await response.json();
      setNpuData(npus);
    };

    fetchNpus();
  }, []);
  return (
    <main className="flex flex-wrap justify-around p-4">
      {npuData.map((npu) => (
        <div
          key={npu.id}
          className="w-92 rounded overflow-hidden shadow-lg m-4 flex flex-col bg-white"
        >
          <img
            className="w-full h-64 object-cover"
            src={npu.imageUrl}
            alt={npu.name}
          />
          <div className="px-6 py-4 flex-grow">
            <div className="font-bold text-xl mb-2">{npu.name}</div>
            <div className="text-gray-700 text-base h-20 overflow-auto">
              {npu.description.substring(0, 100)}...
            </div>
          </div>
          <div className="px-6 pt-2 pb-4 flex justify-between items-center">
            <div className="flex items-center">
              <LightBulbIcon className="h-7 w-7 text-yellow-500 mr-2" />
              <span className="text-sm font-semibold text-gray-700 mr-4">
                {npu.creativity.score ? npu.creativity.score.toFixed(1) : 0}
              </span>
              <FingerPrintIcon className="h-7 w-7 text-yellow-500 mr-2" />
              <span className="text-sm font-semibold text-gray-700">
                {npu.uniqueness.score ? npu.uniqueness.score.toFixed(1) : 0}
              </span>
            </div>
            <Link href={`/npuDetail/${npu.id}`} passHref>
              <button className="flex items-center text-white bg-blue-500 px-3 py-2 rounded">
                <span>Details</span>
                <ChevronRightIcon className="h-5 w-5 ml-2" />
              </button>
            </Link>
          </div>
        </div>
      ))}
    </main>
  );
}
