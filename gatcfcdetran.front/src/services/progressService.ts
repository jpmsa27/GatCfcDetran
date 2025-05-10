import api from './api'

export async function GetProgress(cpf: string) {
  const response = await api.get(`/api/Progress/${cpf}`)
  return response.data
}
