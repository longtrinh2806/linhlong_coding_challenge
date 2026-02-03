import api from '../axios';
import { API_ENDPOINTS } from '../endpoints';

export const healthService = {
  check: () =>
    api.get<string>(API_ENDPOINTS.HEALTH.CHECK),
};
