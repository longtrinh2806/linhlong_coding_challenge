import axios from 'axios';
import { API_CONFIG } from './config';
import { addAuthInterceptor } from './interceptors/request';
import { handleAuthError } from './interceptors/response';

const api = axios.create(API_CONFIG);

api.interceptors.request.use(addAuthInterceptor);
api.interceptors.response.use(undefined, handleAuthError);

export default api;
