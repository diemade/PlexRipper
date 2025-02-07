import { mount } from 'cypress/vue2';

// Augment the Cypress namespace to include type definitions for
// your custom command.
// Alternatively, can be defined in cypress/support/component.d.ts
// with a <reference path="./component" /> at the top of your spec.

type MountParams = Parameters<typeof mount>;
type OptionsParam = MountParams[1];

declare global {
	namespace Cypress {
		interface Chainable {
			mount: typeof mount;
		}
	}
}
