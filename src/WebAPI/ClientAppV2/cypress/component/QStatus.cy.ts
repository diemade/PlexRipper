// import QStatus from '../../src/components/Common/QStatus.vue';
import QStatus from '@components/Common/QStatus.vue';

describe('<QStatus />', () => {
	beforeEach(() => {
		cy.resetNuxt();
	});

	it('Given `CustomPage` When component `mount` Then `body` seen', () => {
		// Arrange
		cy.stubNuxtInject('policyApi', () => Promise.resolve({ body: 'body' }));

		// Act
		cy.mount(QStatus, {
			attrs: {
				props: {
					value: true,
				},
			},
		});
		// Assert
		// cy.get('[data-v-app=""] > div').should('have.text', 'body');
	});
});
