import api from './api'

interface LoginPayload {
  email: string
  password: string
}

export async function AuthUser(payload: LoginPayload) {
  const response = await api.post('/api/Auths', payload)
  return response.data
}
