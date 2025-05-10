import api from './api'

interface CreateSchedule {
  cpf: string,
  scheduleDate: string
}

export async function CreateSchedule(payload: CreateSchedule) {
  const response = await api.post('/api/Schedules', payload)
  return response.data
}

export async function GetSchedules() {
  const response = await api.get('/api/Schedules')
  return response.data
}

export async function GetSchedulesByCpf(cpf: string) {
  const response = await api.get(`/api/Schedules/${cpf}`)
  return response.data
}

export async function GetSchedule(cnpj: string) {
  const response = await api.get(`/api/Schedules/${cnpj}`)
  return response.data
}
