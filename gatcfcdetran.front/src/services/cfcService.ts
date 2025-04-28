import api from './api'

interface CreateCfc {
  name : string,
  cnpj : string,
  address : string,
  email : string,
  cpf : string,
  password : string
}

export async function CreateCfc(payload: CreateCfc) {
  const response = await api.post('/api/Cfcs', payload)
  return response.data
}

export async function GetCfc(cnpj: string) {
  const response = await api.get(`/api/Cfcs/${cnpj}`)
  return response.data
}
