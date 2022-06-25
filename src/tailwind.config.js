/** @type {import('tailwindcss').Config} */
const colors = require('tailwindcss/colors');

module.exports = {
    // This "hack" ensures your IDE detects all normal Tailwind classes, while JIT is used when compiling the project.
    // All the normal Tailwind classes should show up in code completion. It can't show all the new classes generated by JIT.
    mode: process.env.NODE_ENV ? 'jit' : undefined,
    darkMode: 'class',
    content: [
            './Pages/**/*.cshtml',
            './Views/**/*.chstml'
    ],
    theme: {
        colors: {
            primary: 'var(--color-primary)',
            secondary : 'var(--color-secondary)',
            current : colors.current,
            black: colors.black,
            white: colors.white,
            slate: colors.slate,
            gray: colors.gray,
            zinc: colors.zinc,
            neutral: colors.neutral,
            stone: colors.stone,
            red: colors.red,
            orange: colors.orange,
            amber: colors.amber,
            yellow: colors.yellow,
            lime: colors.lime,
            green: colors.green,
            emerald: colors.emerald,
            teal: colors.teal,
            cyan: colors.cyan,
            sky: colors.sky,
            blue: colors.blue,
            indigo: colors.indigo,
            violet: colors.violet,
            purple: colors.purple,
            fuchsia: colors.fuchsia,
            pink: colors.pink,
            rose: colors.rose,
            dark: {
                '100': '#121212',
                '200': '#1d1d1d',
                '300': '#212121',
                '400': '#242424',
                '500': '#272727',
                '600': '#2c2c2c',
                '700': '#2d2d2d',
                '800': '#333333',
                '900': '#353535',
                '999': '#373737',
            }

        },
        minWidth: {
            '0': '0',
            '1/4': '25%',
            '1/2': '50%',
            '3/4': '75%',
            'full': '100%',
        },
        extend: {

        },
    },
    plugins: [
        require('@tailwindcss/forms')
    ],
}