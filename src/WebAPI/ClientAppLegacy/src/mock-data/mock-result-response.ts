import { MockConfig } from '@mock/interfaces/MockConfig';
import ResultDTO from '@dto/ResultDTO';
import { checkConfig } from '@mock/mock-base';

export function generateResultDTO<T>(value: T, config: Partial<MockConfig> = {}): ResultDTO<T> {
	const validConfig = checkConfig(config);

	return {
		value,
		errors: [],
		isSuccess: true,
		isFailed: false,
		reasons: [],
		successes: [],
	};
}
