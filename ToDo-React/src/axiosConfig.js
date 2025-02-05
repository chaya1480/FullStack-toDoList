
  import axios from 'axios';
  import { getToken, logout } from './authService';
  
  const apiClient = axios.create({
      baseURL: "http://localhost:5280",  
      headers: {
        'Content-Type': 'application/json',
      },
  });
  
  apiClient.interceptors.request.use((config) => {
      const token = getToken();
      if (token) {
          config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
  }, (error) => {
    return Promise.reject(error);
  });
  
  apiClient.interceptors.response.use(
    response => response,
    error => {
        if (error.response.status === 401) {
            logout();
            window.location.href = "/login";
        }
        return Promise.reject(error);
    }
);

export default apiClient;
