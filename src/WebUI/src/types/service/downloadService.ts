import { ReplaySubject, Observable } from 'rxjs';
import { getAllDownloads } from '@api/plexDownloadApi';
import { map, switchMap } from 'rxjs/operators';
import GlobalService from '@service/globalService';
import Log from 'consola';
import { DownloadTaskDTO, PlexServerDTO } from '@dto/mainApi';
import HealthService from '@service/healthService';

export class DownloadService {
	private _downloadServerList: ReplaySubject<PlexServerDTO[]> = new ReplaySubject();

	public constructor() {
		GlobalService.getAxiosReady()
			.pipe(switchMap(() => getAllDownloads()))
			.subscribe((value) => {
				Log.debug('Retrieving downloadlist');
				this._downloadServerList.next(value ?? []);
			});

		HealthService.getServerStatus().subscribe((status) => {
			if (status) {
				this.fetchDownloadList();
			}
		});
	}

	/**
	 * returns the downloadTasks nested in PlexServerDTO -> PlexLibraryDTO -> DownloadTaskDTO[]
	 */
	public getDownloadList(): Observable<DownloadTaskDTO[]> {
		return this._downloadServerList
			.asObservable()
			.pipe(map((value) => value.map((x) => x.plexLibraries.map((y) => y.downloadTasks)).flat(2)));
	}

	public getDownloadListInServers(): Observable<PlexServerDTO[]> {
		return this._downloadServerList.asObservable();
	}

	/**
	 * Fetch the download list and signal to the observers that it is done.
	 */
	public fetchDownloadList(): Observable<PlexServerDTO[]> {
		getAllDownloads().subscribe((value) => this._downloadServerList.next(value ?? []));
		return this.getDownloadListInServers();
	}
}

const downloadService = new DownloadService();
export default downloadService;
