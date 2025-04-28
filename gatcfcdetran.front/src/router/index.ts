import { createRouter, createWebHistory } from 'vue-router'

// Importando as páginas
import Login from '../views/LoginPage.vue'
import Home from '../views/Home.vue' // você pode criar esse depois, ou mudar se quiser

const routes = [
  {
    path: '/',
    name: 'Login',
    component: Login
  },
  {
    path: '/home',
    name: 'Home',
    component: Home
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
