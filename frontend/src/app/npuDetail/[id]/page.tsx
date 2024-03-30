"use client";
import { usePathname, useRouter } from "next/navigation";
import { useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import npuData from "@/MOCK_DATA.json";
import { Npu } from "@/models/NpuModel";
import { ChevronLeftIcon } from "@heroicons/react/20/solid";

export default function NpuDetail() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const pathname = usePathname();
  const id = pathname.split("/").pop();
  console.log("ID", id);

  const [npu, setNpu] = useState<Npu | null>(null);

  useEffect(() => {
    if (!id) {
      console.log("NO ID :(");
    }
    if (id) {
      const foundNpu = npuData.find((npu) => {
        console.log(npu.id, id);
        return npu.id === id;
      });
      if (foundNpu) {
        setNpu(foundNpu);
      } else {
        setNpu(null);
      }
    }
  }, [id]);

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
          <p className="text-lg">
            <strong className="font-bold">Creativity:</strong>{" "}
            {npu.creativity.score}
            <strong className="font-bold">Uniqueness:</strong>{" "}
            {npu.uniqueness.score}
          </p>
        </div>
      </div>
    </div>
  );
}
