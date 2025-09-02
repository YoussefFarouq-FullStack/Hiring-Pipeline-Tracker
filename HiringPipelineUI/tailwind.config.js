/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/index.html',
    './src/app/**/*.{html,ts}',
  ],
  theme: {
    extend: {
      fontSize: {
        'xs': ['0.75rem', { lineHeight: '1rem' }],     /* 12px */
        'sm': ['0.875rem', { lineHeight: '1.25rem' }], /* 14px */
        'base': ['0.875rem', { lineHeight: '1.5rem' }], /* 14px */
        'lg': ['1rem', { lineHeight: '1.75rem' }],      /* 16px */
        'xl': ['1.125rem', { lineHeight: '1.75rem' }],  /* 18px */
        '2xl': ['1.25rem', { lineHeight: '2rem' }],    /* 20px */
        '3xl': ['1.5rem', { lineHeight: '2rem' }],     /* 24px */
        '4xl': ['1.875rem', { lineHeight: '2.25rem' }], /* 30px */
        '5xl': ['2.25rem', { lineHeight: '2.5rem' }],  /* 36px */
        '6xl': ['3rem', { lineHeight: '1' }],          /* 48px */
      },
    },
  },
  plugins: [],
};


