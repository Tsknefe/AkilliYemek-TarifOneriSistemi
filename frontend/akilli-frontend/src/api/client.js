// src/api/client.js
import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "https://localhost:7016/api",
  timeout: 15000, // 15 saniye timeout
});

// REQUEST INTERCEPTOR
api.interceptors.request.use(
  (config) => {
    // Gerekirse token buraya eklenir:
    // config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);

// RESPONSE INTERCEPTOR
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Backend hata mesajı varsa oradan al
    if (error.response?.data?.message) {
      return Promise.reject(new Error(error.response.data.message));
    }

    // Yoksa standart error ver
    return Promise.reject(error);
  }
);

export default api;
