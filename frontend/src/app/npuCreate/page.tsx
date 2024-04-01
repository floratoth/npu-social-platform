"use client";
import { useRouter } from "next/navigation";
import { useState, ChangeEvent, FormEvent } from "react";
import { toast } from "react-toastify";

function NpuCreate() {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const router = useRouter();

  const handleImageUpload = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files ? event.target.files[0] : null;
    const reader = new FileReader();

    reader.onloadend = () => {
      setImageUrl(reader.result as string);
    };

    if (file) {
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();

    // Ensure all fields are filled
    if (!name || !description || !imageUrl) {
      alert("Please fill all fields and upload an image.");
      return;
    }

    try {
      const response = await fetch("http://localhost:5105/api/npu", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          name: name,
          description: description,
          imageUrl: imageUrl,
          creativity: { score: 0, votes: 0 },
          uniqueness: { score: 0, votes: 0 },
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      } else {
        const data = await response.json();
        router.push("/");
        setTimeout(
          () =>
            toast.success("Thank you! Your new NPU was successfully saved."),
          1000
        );
      }
    } catch (error) {
      console.error("Error:", error);
    }
  };

  return (
    <div className="min-h-screen flex justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div>
          <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
            Create a New NPU
          </h2>
        </div>
        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          <div className="rounded-md shadow-sm -space-y-px">
            <div>
              <label htmlFor="name" className="sr-only">
                Name
              </label>
              <input
                id="name"
                name="name"
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
                className="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-t-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                placeholder="Name"
              />
            </div>
            <div>
              <label htmlFor="description" className="sr-only">
                Description
              </label>
              <textarea
                id="description"
                name="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                required
                className="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-b-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                placeholder="Description"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">
              Cover photo
            </label>
            <div className="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-md">
              <div className="space-y-1 text-center">
                <input
                  type="file"
                  onChange={handleImageUpload}
                  accept="image/*"
                  className="mt-1"
                  required
                />
              </div>
            </div>
            {imageUrl && (
              <div className="mt-4">
                <img
                  src={imageUrl}
                  alt="Uploaded"
                  className="w-full h-64 object-cover"
                />
              </div>
            )}
          </div>

          <div>
            <button
              type="submit"
              className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Submit
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default NpuCreate;
