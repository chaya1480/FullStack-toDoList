// export function logout() {
//     localStorage.removeItem("token");
// }

// export function getToken() {
//     return localStorage.getItem("token");
// }
import apiClient from './api';  
export const login = async (username, password) => {
    try {
        const response = await apiClient.post('/login', 
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
export async function register(username, password) {
    if (!username || !password) {
        console.error("Username and password are required");
        return;
    }

    try {
        const response = await apiClient.post('/register', { 
            username: username, 
            passwordHash: password  
        });

        const loginResponse = await login({ username, password });
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

apiClient.interceptors.request.use((config) => {
    const token = getToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => Promise.reject(error));
