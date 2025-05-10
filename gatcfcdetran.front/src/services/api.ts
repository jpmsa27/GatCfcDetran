import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5000',
  timeout: 10000, // 10 segundos
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}, error => {
  return Promise.reject(new Error(error))
})

export default api
