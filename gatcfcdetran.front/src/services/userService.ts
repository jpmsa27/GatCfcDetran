import api from './api'

interface CreateUser {
  name: string,
  cpf: string,
  password: string,
  birthDate: string,
  email: string,
}

export async function CreateUser(payload: CreateUser) {
  const response = await api.post('/api/Users', payload)
  return response.data
}

export async function CreateAdmin(payload: CreateUser) {
  const response = await api.post('/api/Users/admin', payload)
  return response.data
}

export async function GetUserById(id: string) {
  const response = await api.get(`/api/Users/${id}`)
  return response.data
}

export async function GetUser(cpf: string) {
  const response = await api.get(`/api/Users/cpf/${cpf}`)
  return response.data
}

export async function GetUsers() {
  const response = await api.get('/api/Users')
  return response.data
}