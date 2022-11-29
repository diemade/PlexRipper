import { defineConfig } from 'cypress';

export default defineConfig({
	component: {
		viewportHeight: 1080,
		viewportWidth: 960,
		devServer: {
			framework: 'nuxt',
			bundler: 'webpack',
		},
		specPattern: 'src/tests/components/**/*.spec.cy.ts',
	},

	e2e: {
		viewportWidth: 1920,
		viewportHeight: 1080,
		baseUrl: 'http://localhost:3030',
	},
});