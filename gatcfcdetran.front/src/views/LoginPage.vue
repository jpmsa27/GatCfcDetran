<template>
  <div class="login-container">
    <form class="login-form" @submit.prevent="handleLogin">
      <h2>Bem-vindo de volta!</h2>

      <div class="input-group">
        <label for="email">Email</label>
        <input id="email" type="email" v-model="email" required />
      </div>

      <div class="input-group">
        <label for="password">Senha</label>
        <input id="password" type="password" v-model="password" required />
      </div>

      <button type="submit">Entrar</button>

      <p class="signup-text">Ainda não tem conta? <a href="#">Cadastre-se</a></p>
    </form>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref } from 'vue'
  import { useRouter } from 'vue-router'
  import { AuthUser } from '../services/authService'


export default defineComponent({
  setup() {
    const email = ref('')
    const password = ref('')
    const router = useRouter()

    const handleLogin = async () => {
      try {
        const payload = {
          email: email.value,
          password: password.value
        }
        const response = await AuthUser(payload)

        console.log('Login sucesso:', response)

        // Aqui você pode guardar o token, etc.
        localStorage.setItem('token', response.token)

        // Redirecionar para a Home
        router.push('/home')

      } catch (error: any) {
        console.error('Erro no login:', error)
        alert('Falha no login. Verifique seus dados.')
      }
    }

    return {
      email,
      password,
      handleLogin
    }
  }
})
</script>

<style scoped>
  .login-container {
    min-height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    background: linear-gradient(135deg, #a8e6cf, #dcedc1);
  }

  .login-form {
    background: white;
    padding: 2rem;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 128, 0, 0.2);
    width: 100%;
    max-width: 400px;
  }

  h2 {
    margin-bottom: 1.5rem;
    color: #388e3c;
    text-align: center;
  }

  .input-group {
    margin-bottom: 1rem;
  }

  label {
    display: block;
    margin-bottom: 0.5rem;
    font-weight: bold;
    color: #2e7d32;
  }

  input {
    width: 100%;
    padding: 0.75rem;
    border: 1px solid #c8e6c9;
    border-radius: 8px;
    background: #f1f8e9;
    transition: border-color 0.3s;
  }

    input:focus {
      border-color: #66bb6a;
      outline: none;
    }

  button {
    width: 100%;
    padding: 0.75rem;
    margin-top: 1rem;
    background: #4caf50;
    border: none;
    border-radius: 8px;
    color: white;
    font-size: 1rem;
    font-weight: bold;
    cursor: pointer;
    transition: background 0.3s;
  }

    button:hover {
      background: #43a047;
    }

  .signup-text {
    margin-top: 1rem;
    text-align: center;
    color: #558b2f;
  }

    .signup-text a {
      color: #2e7d32;
      font-weight: bold;
      text-decoration: none;
    }

      .signup-text a:hover {
        text-decoration: underline;
      }
</style>
