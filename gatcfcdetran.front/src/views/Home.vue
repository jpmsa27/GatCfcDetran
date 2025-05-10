<template>
  <h1 class="header-container">Bem-vindo, {{ userProfile?.name }}</h1>
  <div class="home-container">

    <div v-if="isSuperOrAdmin">
      <div class="actions">
        <button v-if="isSuper === 'SUPER'" @click="createAdmin">Criar Admin</button>
        <button v-if="isAdmin === 'ADMIN'|| isSuper === 'SUPER'" @click="createUser">Criar Usuário</button>
        <button v-if="isAdmin === 'ADMIN'|| isSuper === 'SUPER'" @click="listUsers">Listar Usuários</button>

        <div v-if="userList.length > 0" class="user-list">
          <h2>Lista de Usuários</h2>
          <ul>
            <li v-for="user in userList" :key="user.id">
              {{ user.name }} ({{ user.cpf }}) - {{ user.email }} - {{ user.role === 0 ? 'Usuário' : user.role === 1 ? 'Admin' : 'Super Admin' }}
            </li>
          </ul>
        </div>

        <div v-if=createUserContainer class="create-user-container">
          <h2>Criar Usuário</h2>
          <input type="text" v-model="userName" placeholder="Nome do usuário" required />
          <input type="text" v-model="userCpf" placeholder="CPF do usuário" required />
          <input type="email" v-model="userEmail" placeholder="Email do usuário" required />
          <input type="date" v-model="userBirthDate" placeholder="Data de Nascimento" required />
          <input type="password" v-model="userPassword" placeholder="Senha do usuário" required />
          <button type="button" @click="createUserRequest">Criar</button>
        </div>

        <div v-if="createAdminContainer" class="create-admin-container">
          <h2>Criar Admin</h2>
          <input type="text" v-model="adminName" placeholder="Nome do admin" required />
          <input type="text" v-model="adminCpf" placeholder="CPF do admin" required />
          <input type="email" v-model="adminEmail" placeholder="Email do admin" required />
          <input type="date" v-model="adminBirthDate" placeholder="Data de Nascimento" required />
          <input type="password" v-model="adminPassword" placeholder="Senha do admin" required />
          <button type="button" @click="createAdminRequest">Criar</button>
        </div>

        <div v-if="isAdmin === 'ADMIN' || isSuper === 'SUPER'" class="procurar-usuario">
          <h2>Procurar Usuário</h2>
          <input type="text"  v-model="cpf" required placeholder="Digite o cpf do usuário." />
          <button type="button" @click="handleSearch" >Buscar</button>
        </div>
      </div>
    </div>

    <div v-if="isUser === 'USER'" class='loged-user-container'>
      <h2>Suas Avaliações</h2>
      <ul>
        <li v-for="evaluation in evaluations" :key="evaluation.id">
          Avaliação com ID: {{ evaluation.id }} - {{ evaluation.scheduleDate }} - {{ evaluation.done ? 'Concluída' : 'Pendente' }}
        </li>
      </ul>
    </div>

    <div v-if="usuario" class="user-details">
      <h3>Dados do Usuário</h3>
      <p><strong>ID:</strong> {{ usuario.id }}</p>
      <p><strong>Nome:</strong> {{ usuario.nome }}</p>
      <p><strong>CPF:</strong> {{ usuario.cpf }}</p>
      <p><strong>Email:</strong> {{ usuario.email }}</p>
      <p><strong>Data de Nascimento:</strong> {{ usuario.birthDate }}</p>

      <h3 v-if="isUser != 'USER'">Marcar Avaliação</h3>
      <input v-if="isUser != 'USER'" type="date" v-model="avaliacaoDate" required />

      <button v-if="isUser != 'USER'" @click="marcarExame(usuario.cpf)">Marcar Exame</button>
      <button v-if="isUser != 'USER'" @click="userProgress(usuario.cpf)">Ver Progresso</button>
      <div v-if="evaluations.length > 0" class="user-progress">
        <h3>Progresso do Usuário</h3>
        <h4>Total de avaliações completas: {{ evaluations.filter(item => item.done === true).length }}</h4>
        <h4>Total de exames válidos marcados: {{ evaluations.filter(item => item.done === false && new Date(item.scheduleDate).setHours(0,0,0,0) > new Date().setHours(0,0,0,0)).length }}</h4>
        <h4>Aulas mínimas: {{ progress.aulasMinimas }}</h4>
        <ul>
          <li v-for="item in evaluations" :key="item.id">
            <strong>{{ item.id }}</strong>
            <ul>{{ item.scheduleDate }}</ul>
            <ul>{{ item.done }}</ul>
          </li>
        </ul>
      </div>
      <button class="botaoFechar" v-if="isUser != 'USER'" @click="fecharAba">Fechar</button>
    </div>

  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, onMounted, onBeforeUnmount } from 'vue'
  import { CreateUser, GetUser, CreateAdmin, GetUserById, GetUsers } from '../services/userService'
  import { GetSchedules, CreateSchedule, GetSchedulesByCpf } from '../services/scheduleService'
  import { GetProgress } from '../services/progressService'

export default defineComponent({
  setup() {
    const userProfile = ref<any>(null)
    const userList = ref<any[]>([])
    const evaluations = ref<any[]>([])
    const progress = ref('');
    const usuario = ref<any>(null);
    const createUserContainer = ref(false)
    const createAdminContainer = ref(false)
    const avaliacaoDate = ref('')


    const cpf = ref('')
    const isSuper = ref(false)
    const isAdmin = ref(false)
    const isUser = ref(false)
    const userName = ref('')
    const userCpf = ref('')
    const userEmail = ref('')
    const userBirthDate = ref('')
    const userPassword = ref('')
    const adminName = ref('')
    const adminCpf = ref('')
    const adminEmail = ref('')
    const adminBirthDate = ref('')
    const adminPassword = ref('')


    onMounted(async () => {
      try {
        const userId = localStorage.getItem('userId') // Ou pegue do token decodificado
        if (!userId) throw new Error('Usuário não autenticado')

        userProfile.value = await GetUserById(userId)

        console.log('Perfil do usuário:', userProfile.value)
        // Verifica o tipo
        isSuper.value = userProfile.value.role === 2 ? 'SUPER' : 'UNKNOWN'
        isAdmin.value = userProfile.value.role === 1 ? 'ADMIN' : 'UNKNOWN'
        isUser.value = userProfile.value.role === 0 ? 'USER' : 'UNKNOWN'

        if(isUser.value === 'USER'){
          const response = await GetSchedulesByCpf(userProfile.value.cpf)
          console.log('Avaliações do usuário:', response)
          const responseUser = await GetUser(userProfile.value.cpf)
          console.log('Usuário encontrado:', responseUser)
          usuario.value = {
            nome: responseUser.name,
            cpf: responseUser.cpf,
            email: responseUser.email,
            id: responseUser.id,
            birthDate: responseUser.birthDate,
          };
          userProgress(userProfile.value.cpf)
          

          evaluations.value = response
        } 
        else {
          console.error('Tipo de usuário desconhecido')
        }

      } catch (error) {
        console.error('Erro ao carregar dados:', error)
      }
    })

    const handleSearch = async () => {
      try {
        if (!cpf.value) {
          alert('Por favor, insira um CPF.')
          return
        }
        const response = await GetUser(cpf.value)
        console.log('Usuário encontrado:', response)
        usuario.value = {
          nome: response.name,
          cpf: response.cpf,
          email: response.email,
          id: response.id,
          birthDate: response.birthDate,
        };
      }
      catch (error) {
        console.error('Erro ao buscar usuário:', error)
        alert('Usuário não encontrado.')
      }
    }

    const createAdmin = () => {
      createAdminContainer.value = true
      createUserContainer.value = false
    }

    const createUser = () => {
      createUserContainer.value = true
      createAdminContainer.value = false
    }

    const createUserRequest = async () => {
      try {
        if (!userName.value || !userCpf.value || !userEmail.value) {
          alert('Por favor, preencha todos os campos.')
          return
        }
        const birthDate = new Date(userBirthDate.value)
        console.log('Data de Nascimento:', birthDate)
        const payload = {
          name: userName.value,
          cpf: userCpf.value,
          email: userEmail.value,
          birthDate: new Date(Date.UTC(birthDate.getFullYear(), birthDate.getMonth(), birthDate.getDate())),
          password: userPassword.value,
        }
        console.log('Payload:', payload)
        const response = await CreateUser(payload)
        console.log('Usuário criado:', response)
        alert('Usuário criado com sucesso!')
        createUserContainer.value = false
        usuario.value = {
          nome: response.name,
          cpf: response.cpf,
          email: response.email,
          id: response.id,
          birthDate: response.birthDate,
        };
      } catch (error) {
        console.error('Erro ao criar usuário:', error)
        alert('Erro ao criar usuário.')
      }
    }
    
    const createAdminRequest = async () => {
      try {
        if (!adminName.value || !adminCpf.value || !adminEmail.value) {
          alert('Por favor, preencha todos os campos.')
          return
        }
        const birthDate = new Date(adminBirthDate.value)
        console.log('Data de Nascimento:', birthDate)
        const payload = {
          name: adminName.value,
          cpf: adminCpf.value,
          email: adminEmail.value,
          birthDate: new Date(Date.UTC(birthDate.getFullYear(), birthDate.getMonth(), birthDate.getDate())),
          password: adminPassword.value,
        }
        console.log('Payload:', payload)
        const response = await CreateAdmin(payload)
        console.log('Usuário criado:', response)

        createAdminContainer.value = false
        usuario.value = {
          nome: response.name,
          cpf: response.cpf,
          email: response.email,
          id: response.id,
          birthDate: response.birthDate,
        };

      } catch (error) {
        console.error('Erro ao criar usuário:', error)
        alert('Erro ao criar usuário.')
      }
    }

    const markEvaluation = (userId: string) => {
      alert(`Marcar avaliação para o usuário ${userId}`)
    }

    const marcarExame = (userCpf: string) => {
      const evaluationDate = new Date(avaliacaoDate.value)

      const payload = {
        cpf: userCpf,
        scheduleDate: new Date(Date.UTC(evaluationDate.getFullYear(), evaluationDate.getMonth(), evaluationDate.getDate())),
      }
      console.log('Payload para marcar exame:', payload)
      CreateSchedule(payload).then((response) => {
        console.log('Exame marcado:', response)
        alert('Exame marcado com sucesso!')
      }).catch((error) => {
        console.error('Erro ao marcar exame:', error)
        alert('Erro ao marcar exame.')
      })
    }

    const fecharAba = (userId: string) => {
      usuario.value = null
    }
    
    const computed = () => {
      return {
        isSuperOrAdmin: isSuper.value || isAdmin.value
      }
    }

    const userProgress = async (cpf: string) => {
      try {
        const response = await GetSchedulesByCpf(cpf)
        console.log('Progresso do usuário:', response)
        evaluations.value = response

        const presponseProgress = await GetProgress(cpf)
        console.log('Progresso do usuário:', presponseProgress)
        progress.value = presponseProgress

      } catch (error) {
        console.error('Erro ao buscar progresso:', error)
      }
    }

    const listUsers = async () => {
      try {
        const response = await GetUsers()
        console.log('Lista de usuários:', response)
        userList.value = response
      } catch (error) {
        console.error('Erro ao listar usuários:', error)
      }
    }

    return {
      fecharAba,
      usuario,
      handleSearch,
      cpf,
      userProfile,
      userList,
      evaluations,
      isSuper,
      isAdmin,
      isUser,
      isSuperOrAdmin: computed(() => isSuper.value || isAdmin.value),
      createAdmin,
      createUser,
      markEvaluation,
      createUserContainer,
      createUserRequest,
      userName,
      userCpf,
      userEmail,
      userBirthDate,
      userPassword,
      createAdminContainer,
      createAdminRequest,
      adminName,
      adminCpf,
      adminEmail,
      adminBirthDate,
      adminPassword,
      marcarExame,
      listUsers,
      avaliacaoDate,
      userProgress,
      progress,
    }
  }
})
</script>

<style scoped>

  .user-list {
    margin-top: 2rem;
    margin-left: -1rem;
    padding: 1rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 128, 0, 0.2);
    width: 100%;
    max-width: 500px;
  }

  .user-list ul {
    list-style-type: none;
    padding: 0;
  }
  .user-list li {
    margin-bottom: 1rem;
    padding: 0.5rem;
    background: #e0ffe9;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 128, 0, 0.5);
  }
  .user-list li:hover {
    background: #e0f7fa;
    cursor: pointer;
  }
  
  .create-admin-container {
    margin-top: 2rem;
    margin-left: 2rem;
    padding: 1rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 128, 0, 0.2);
    width: 100%;
    max-width: 300px;
  }

  .loged-user-container {
    margin-top: 2rem;
    margin-left: 2rem;
    margin-right: 2rem;
    padding: 1rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 128, 0, 0.2);
    width: auto;
    max-width: 800px;
  }

  .loged-user-container li {
    list-style-type: none;
    padding: 0.35rem;
  }

  .create-admin-container input {
    margin-right: 1rem;
    margin-bottom: 1rem;
    padding: 0.5rem;
    border: 1px solid #ccc;
    border-radius: 8px; /* Ajusta a largura do input */
    width: 80%;
  }
  .create-user-container {
    margin-top: 2rem;
    margin-left: 2rem;
    padding: 1rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 128, 0, 0.2);
    width: 100%;
    max-width: 300px;
  }
  
  .create-user-container input {
    margin-right: 1rem;
    margin-bottom: 1rem;
    padding: 0.5rem;
    border: 1px solid #ccc;
    border-radius: 8px; /* Ajusta a largura do input */
    width: 80%;
  }

  .botaoFechar {
    margin-top: 1rem;
    padding: 0.5rem 1rem;
    background: #f44336;
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: bold;
    cursor: pointer;
  }
  
  .user-details {
    margin-top: 1rem;
    padding: 1rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 128, 0, 0.2);
    width: auto;
    max-width: 360px;
  }

  .pro {
    margin: 1rem;
  }

  .user-details input{
    margin-right: 1rem;
    margin-bottom: 1rem;
    padding: 0.5rem;
    border: 1px solid #ccc;
    border-radius: 8px; /* Ajusta a largura do input */
    width: 80%;
  }

  .procurar-usuario {
    margin: 2rem 0;
    padding: 1rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 128, 0, 0.2);
    width: 100%;
    max-width: 600px;
  }
  
  .procurar-usuario input {
    margin-right: 1rem;
    padding: 0.5rem;
    border: 1px solid #ccc;
    border-radius: 8px; /* Ajusta a largura do input */
  }

  .header-container {
    text-align: left;
    min-height: 5vh;
    vertical-align: middle;
    margin: 0%;
    padding-top: 1vh;
    padding-left: 1vh;
    background: linear-gradient(135deg, #a8e6cf, #dcedc1);
  }
  .home-container {
    min-height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    background: linear-gradient(135deg, #a8e6cf, #dcedc1);
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
