import IStoreState from '@interfaces/service/IStoreState';
import IAppConfig from '@class/IAppConfig';
import {
	ConfirmationSettingsDTO,
	DateTimeSettingsDTO,
	DisplaySettingsDTO,
	DownloadManagerSettingsDTO,
	GeneralSettingsDTO,
	LanguageSettingsDTO,
	ServerSettingsDTO,
} from '@dto/mainApi';

const defaultState: IStoreState = {
	config: {} as IAppConfig,
	pageReady: false,
	accounts: [],
	servers: [],
	serverConnections: [],
	libraries: [],
	mediaUrls: [],
	notifications: [],
	folderPaths: [],
	alerts: [],
	helpIdDialog: '',
	downloadTaskUpdateList: [],
	// Progress Service
	fileMergeProgressList: [],
	inspectServerProgress: [],
	syncServerProgress: [],
	libraryProgress: [],
	serverDownloads: [],
	serverConnectionCheckStatusProgress: [],
	jobStatus: [],
	// Settings Modules
	dateTimeSettings: {} as DateTimeSettingsDTO,
	downloadManagerSettings: {} as DownloadManagerSettingsDTO,
	generalSettings: {} as GeneralSettingsDTO,
	languageSettings: {} as LanguageSettingsDTO,
	displaySettings: {} as DisplaySettingsDTO,
	confirmationSettings: {} as ConfirmationSettingsDTO,
	serverSettings: {} as ServerSettingsDTO,
};

export default defaultState;
