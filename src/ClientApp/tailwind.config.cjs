/** @type {import('tailwindcss').Config} */
const defaultTheme = require("tailwindcss/defaultTheme");
module.exports = {
  darkMode: "class",
  content: ["./src/**/*.{js,jsx,ts,tsx}"],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Inter var", ...defaultTheme.fontFamily.sans],
      },
      colors: {
        primary: {
          DEFAULT: "#005296",
          DEFAULT2: "#0071CE",
          50: "#D8EDFF",
          100: "#C4E4FF",
          200: "#9BD2FF",
          300: "#72BFFF",
          400: "#49ADFF",
          500: "#219BFF",
          600: "#0087F7",
          700: "#0071CE",
          800: "#005296",
          900: "#00335E",
        },
      },
    },
  },
  plugins: [require("@headlessui/tailwindcss")],
};
