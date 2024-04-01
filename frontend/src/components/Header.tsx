import Link from "next/link";

function Header() {
  return (
    <header className="bg-white text-black px-4 py-2 md:px-8 md:py-4 flex justify-between items-center shadow-md">
      <Link href="/">
        <span className="cursor-pointer text-lg md:text-2xl font-bold text-red-600">
          BrickBenders
        </span>
      </Link>
      <nav className="space-x-2 md:space-x-4">
        <Link href="/">
          <span className="cursor-pointer text-sm md:text-lg font-semibold hover:text-red-600">
            NPU List
          </span>
        </Link>
        <Link href="/npuCreate">
          <span className="cursor-pointer text-sm md:text-lg font-semibold hover:text-red-600">
            Create NPU
          </span>
        </Link>
      </nav>
    </header>
  );
}

export default Header;
