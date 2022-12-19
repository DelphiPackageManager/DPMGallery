/** @type {import('tailwindcss').Config} */
const defaultTheme = require('tailwindcss/defaultTheme')
module.exports = {
  darkMode: "class",
  content: [
    "./src/**/*.{js,jsx,ts,tsx}"
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter var', ...defaultTheme.fontFamily.sans],
      },
      colors: {
        primary: {
          DEFAULT: '#005296',
          DEFAULT2: '#0071CE',
          '50': '#D8EDFF',
          '100': '#C4E4FF',
          '200': '#9BD2FF',
          '300': '#72BFFF',
          '400': '#49ADFF',
          '500': '#219BFF',
          '600': '#0087F7',
          '700': '#0071CE',
          '800': '#005296',
          '900': '#00335E'
        },
        /*
        'gray': {
          DEFAULT: '#121212',
          '50': '#BFBFBF',
          '100': '#B5B5B5',
          '200': '#A1A1A1',
          '300': '#8C8C8C',
          '400': '#787878',
          '500': '#646464',
          '600': '#4F4F4F',
          '700': '#3B3B3B',
          '800': '#262626',
          '900': '#121212'
        },
*/
      }
    },
  },
  plugins: [
    require('@headlessui/tailwindcss')
  ],
}
