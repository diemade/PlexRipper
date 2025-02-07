<template>
	<!--	Instead of multiple layouts we merge into one default layout to prevent full
      page change (flashing white background) during transitions.	-->
	<v-app>
		<page-load-overlay v-if="isLoading" :value="isLoading" />
		<template v-else>
			<help-dialog :id="helpId" :show="helpDialogState" @close="helpDialogState = false" />
			<alert-dialog v-for="(alertItem, i) in alerts" :key="i" :alert="alertItem" @close="closeAlert" />
			<CheckServerConnectionsProgress />
			<!--	Use for setup-layout	-->
			<template v-if="isSetupPage">
				<vue-scroll>
					<v-main class="no-background">
						<nuxt />
					</v-main>
				</vue-scroll>
			</template>
			<!--	Use for everything else	-->
			<template v-else>
				<app-bar @show-navigation="toggleNavigationsDrawer" @show-notifications="toggleNotificationsDrawer" />
				<navigation-drawer :show-drawer="showNavigationDrawerState" />
				<notifications-drawer :show-drawer="showNotificationsDrawerState" @cleared="toggleNotificationsDrawer" />
				<v-main>
					<nuxt />
				</v-main>
				<footer />
			</template>
		</template>
		<Background :hide-background="isNoBackground" />
	</v-app>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { useSubscription } from '@vueuse/rxjs';
import Log from 'consola';
import PageLoadOverlay from '@components/General/PageLoadOverlay.vue';
import { AlertService, GlobalService, HelpService } from '@service';
import IAlert from '@interfaces/IAlert';
import NotificationsDrawer from '@overviews/NotificationsDrawer.vue';
import CheckServerConnectionsProgress from '@components/Progress/CheckServerConnectionsProgress.vue';

@Component<Default>({
	components: { InspectServerProgress: CheckServerConnectionsProgress, NotificationsDrawer, PageLoadOverlay },
	loading: false,
})
export default class Default extends Vue {
	isLoading: boolean = true;
	helpDialogState: boolean = false;
	helpId: string = '';
	alerts: IAlert[] = [];
	showNavigationDrawerState: Boolean = true;
	showNotificationsDrawerState: Boolean = false;

	get isSetupPage(): boolean {
		return this.$route.fullPath.includes('setup');
	}

	get isNoBackground(): boolean {
		if (this.isLoading) {
			return true;
		}
		return this.isSetupPage;
	}

	closeAlert(alert: IAlert): void {
		AlertService.removeAlert(alert.id);
	}

	toggleNavigationsDrawer() {
		this.showNavigationDrawerState = !this.showNavigationDrawerState;
	}

	toggleNotificationsDrawer() {
		this.showNotificationsDrawerState = !this.showNotificationsDrawerState;
	}

	mounted(): void {
		useSubscription(
			GlobalService.getPageSetupReady().subscribe(() => {
				Log.debug('Loading has finished, displaying page now');
				this.isLoading = false;
			}),
		);

		useSubscription(
			HelpService.getHelpDialog().subscribe((helpId) => {
				if (helpId) {
					this.helpId = helpId;
					this.helpDialogState = true;
				}
			}),
		);

		useSubscription(
			AlertService.getAlerts().subscribe((alerts) => {
				if (alerts) {
					this.alerts = alerts;
				}
			}),
		);
	}
}
</script>
