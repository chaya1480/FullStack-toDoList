import axios from 'axios';

const apiClient = axios.create({
    baseURL: process.env.REACT_APP_API_URL || "http://localhost:5280",  
    headers: { 'Content-Type': 'application/json' }
});
console.log("API URL:", process.env.REACT_APP_API_URL);
export default apiClient;
