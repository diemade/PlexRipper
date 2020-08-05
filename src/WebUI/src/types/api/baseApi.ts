import Log from 'consola';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { AxiosResponse } from 'axios';
import Result from 'fluent-type-results';

export const baseUrl = 'https://localhost:5001';
// export const baseUrlHttp = "http://localhost:5000";

export const baseApiUrl = `${baseUrl}/api`;

export const signalRDownloadProgressUrl = `${baseUrl}/download/progress`;

export function preApiRequest(logText: string, fnName: string): void {
	Log.debug(`${logText} ${fnName} => sending request`);
}

export function checkResponse<T>(response: Observable<AxiosResponse<Result<T>>>, logText: string, fnName: string): Observable<T> {
	// Pipe response
	return response.pipe(
		tap((res) => {
			if (res.status !== 200) {
				switch (res.status) {
					case 400:
						Log.error(`${logText}${fnName} => Bad Request from response:`, res.request);
						return;

					case 404:
						Log.error(`${logText}${fnName} => Not Found from response:`, res.request);
						return;

					case 500:
						Log.error(`${logText}${fnName} => Internal Server Error from response:`, res.request);
						return;

					default:
						Log.error(`${logText}${fnName} => Unknown Error (Status ${res.status}) from response:`, res.request);
						break;
				}
			}
		}),
		map((res: AxiosResponse) => res.data?.value),
		tap((data) => Log.debug(`${logText}${fnName} response:`, data)),
	);
}