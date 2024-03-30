import npuData from "@/MOCK_DATA.json";
import Link from "next/link";
import {
  ChevronRightIcon,
  LightBulbIcon,
  FingerPrintIcon,
} from "@heroicons/react/20/solid";

export default function Home() {
  return (
    <main className="flex flex-wrap justify-around p-4">
      {npuData.map((npu) => (
        <div
          key={npu.id}
          className="max-w-sm rounded overflow-hidden shadow-lg m-4 flex flex-col bg-white"
        >
          <img
            className="w-full h-64 object-cover"
            src={npu.imageUrl}
            alt={npu.name}
          />
          <div className="px-6 py-4 flex-grow">
            <div className="font-bold text-xl mb-2">{npu.name}</div>
            <p className="text-gray-700 text-base">
              {npu.description.substring(0, 100)}...
            </p>
          </div>
          <div className="px-6 pt-2 pb-4 flex justify-between items-center">
            <div className="flex items-center">
              <LightBulbIcon className="h-7 w-7 text-yellow-500 mr-2" />
              <span className="text-sm font-semibold text-gray-700 mr-4">
                {npu.creativity.score}
              </span>
              <FingerPrintIcon className="h-7 w-7 text-yellow-500 mr-2" />
              <span className="text-sm font-semibold text-gray-700">
                {npu.uniqueness.score}
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
