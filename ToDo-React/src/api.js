// // export const API_URL = process.env.REACT_APP_API_URL;
// const API_BASE_URL =
//   process.env.NODE_ENV === "production"
//     ? "https://your-api.onrender.com" // ה-API ב-Rendre
//     : process.env.REACT_APP_API_URL; // ה-API המקומי בזמן פיתוח

// export const API_URL = API_BASE_URL;

import axios from 'axios';

const apiClient = axios.create({
    baseURL: "http://localhost:5280" || process.env.REACT_APP_API_URL ,  
    headers: { 'Content-Type': 'application/json' }
});
console.log("API URL:", process.env.REACT_APP_API_URL);

export default apiClient;
