import apiClient from './axiosConfig'; // לוודא שזה ייבוא נכון!

// הוספת Interceptor לתפיסת שגיאות
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    console.error('API Error:', error.response ? error.response.data : error.message);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    console.log("Calling getTasks..."); // בדיקה
    const result = await apiClient.get('/tasks');
    return result.data;
  },

  addTask: async (name) => {
    const result = await apiClient.post('/tasks', { name });
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    const result = await apiClient.put(`/tasks/${id}`, { isComplete });
    return result.data;
  },

  deleteTask: async (id) => {
    console.log('deleteTask', id);
    const result = await apiClient.delete(`/tasks/${id}`);
    return result.data;
  },
};
