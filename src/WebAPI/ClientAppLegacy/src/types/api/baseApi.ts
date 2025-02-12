import Log from 'consola';
import { switchMap, take, tap } from 'rxjs/operators';
import { catchError, Observable, of } from 'rxjs';
import { AxiosObservable } from 'axios-observable';
import { AxiosError, AxiosResponse } from 'axios';
import { AlertService } from '@service';
import ResultDTO from '@dto/ResultDTO';

export function checkForError<T = any>(
	logText?: string,
	fnName?: string,
): (source$: AxiosObservable<ResultDTO<T>>) => Observable<ResultDTO<T>> {
	return (source$) =>
		source$.pipe(
			catchError((error: AxiosError | any) => {
				Log.error('FATAL NETWORK ERROR: ', error);

				AlertService.showAlert({ id: 0, title: 'Network Error', text: error, result: error });

				// TODO Check wat the error contains in-case, of network failure and continue based on that
				return of({
					data: { isSuccess: false, isFailed: true, errors: [{ message: error.message }] } as ResultDTO,
				} as AxiosResponse<ResultDTO<T>>);
			}),
			switchMap((response: AxiosResponse<ResultDTO<T>>): Observable<ResultDTO<T>> => {
				return of(response?.data);
			}),
			tap((data) => Log.trace(`${logText}${fnName} response:`, data)),
			// Ensure we complete any API calls after the response has been received
			take(1),
		);
}
