<template>
  <div class="home-container">
    <h1>Bem-vindo, {{ userProfile?.name }}</h1>

    <div v-if="isSuperOrAdmin">
      <div class="actions">
        <button v-if="isSuper" @click="createAdmin">Criar Admin</button>
        <button @click="createUser">Criar Usuário</button>
      </div>

      <h2>Seus Usuários</h2>
      <ul>
        <li v-for="user in userList" :key="user.id">
          {{ user.name }}
          <button @click="markEvaluation(user.id)">Marcar Avaliação</button>
        </li>
      </ul>
    </div>

    <div v-else>
      <h2>Suas Avaliações</h2>
      <ul>
        <li v-for="evaluation in evaluations" :key="evaluation.id">
          Avaliação de {{ evaluation.title }}
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, onMounted } from 'vue'
  import { CreateUser, GetUser } from '../services/userService'
  import { GetSchedules } from '../services/scheduleService'

export default defineComponent({
  setup() {
    const userProfile = ref<any>(null)
    const userList = ref<any[]>([])
    const evaluations = ref<any[]>([])

    const isSuper = ref(false)
    const isAdmin = ref(false)
    const isUser = ref(false)

    onMounted(async () => {
      try {
        const userId = localStorage.getItem('userId') // Ou pegue do token decodificado
        if (!userId) throw new Error('Usuário não autenticado')

        userProfile.value = await GetUser(userId)

        // Verifica o tipo
        isSuper.value = userProfile.value.type === 'SUPER'
        isAdmin.value = userProfile.value.type === 'ADMIN'
        isUser.value = userProfile.value.type === 'USER'

        if (isSuper.value || isAdmin.value) {
          userList.value = await getUserList(userId)
        } else if (isUser.value) {
          evaluations.value = await getEvaluationsByUser(userId)
        }
      } catch (error) {
        console.error('Erro ao carregar dados:', error)
      }
    })

    const createAdmin = () => {
      alert('Criar Admin - aqui abriria um formulário')
    }

    const createUser = () => {
      alert('Criar Usuário - aqui abriria um formulário')
    }

    const markEvaluation = (userId: string) => {
      alert(`Marcar avaliação para o usuário ${userId}`)
    }

    return {
      userProfile,
      userList,
      evaluations,
      isSuper,
      isAdmin,
      isUser,
      isSuperOrAdmin: computed(() => isSuper.value || isAdmin.value),
      createAdmin,
      createUser,
      markEvaluation
    }
  }
})
</script>

<style scoped>
  .home-container {
    padding: 2rem;
  }

  .actions {
    margin-bottom: 2rem;
  }

  button {
    margin-right: 1rem;
    padding: 0.5rem 1rem;
    background: #4caf50;
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: bold;
    cursor: pointer;
  }

    button:hover {
      background: #43a047;
    }
</style>
