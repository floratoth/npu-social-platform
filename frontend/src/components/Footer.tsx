function Footer() {
  return (
    <footer className="bg-red-600 text-white px-4 py-2 md:px-8 md:py-4 flex justify-between items-center">
      <div className="text-sm md:text-lg font-semibold">
        © {new Date().getFullYear()} NPU
      </div>
      <div className="space-x-2 md:space-x-4">
        <span className="cursor-pointer hover:text-yellow-500">
          Terms of Service
        </span>
        <span className="cursor-pointer hover:text-yellow-500">
          Privacy Policy
        </span>
      </div>
    </footer>
  );
}

export default Footer;
