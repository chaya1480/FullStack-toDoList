import axios from 'axios';

const API_URL = "http://localhost:5280";


export const login = async (username, password) => {
    try {
        const response = await axios.post(`${API_URL}/login`, 
            { username, password }, 
            {
                headers: { "Content-Type": "application/json" }
            }
        );
        localStorage.setItem("token", response.data.token);
        return response.data;
    } catch (error) {
        console.error("Login failed:", error.response ? error.response.data : error.message);
        throw error;
    }
};

const apiClient = axios.create({
    baseURL: "http://localhost:5280",
    headers: { 'Content-Type': 'application/json' }
});

apiClient.interceptors.request.use((config) => {
    const token = getToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => Promise.reject(error));

export default apiClient;

export async function register(username, password) {
    if (!username || !password) {
        console.error("Username and password are required");
        return;
    }

    try {
        const response = await axios.post(`${API_URL}/register`, { 
            username: username, 
            passwordHash: password  
        });

        const loginResponse = await login(username, password);
        window.location.href = "/tasks";

        return loginResponse;
    } catch (error) {
        console.error("Registration failed:", error.response?.data || error.message);
        throw error;
    }
}



export function logout() {
    localStorage.removeItem("token");
}

export function getToken() {
    return localStorage.getItem("token");
}