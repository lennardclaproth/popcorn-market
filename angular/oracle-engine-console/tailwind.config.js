/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './src/app/**/*.{html,ts}', // your main app
        './src/lib/**/*.{html,ts}', // your shared/lib components
    ],
    theme: {
        extend: {},
    },
    plugins: [],
};
