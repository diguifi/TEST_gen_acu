<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import LogRegister from './LogRegister.vue'
import ExecutionReport from './ExecutionReport.vue'

const form = ref(null)
const submitted = ref(false)
const connectionStatus = ref('connected')
const ativo = ref('')
const tipo = ref('')
const quantidade = ref('')
const preco = ref('')
const logs = ref('')
const ws = ref(null)
const executionReport = ref({})

const adicionarLog = (mensagem) => {
  const timestamp = new Date().toLocaleTimeString('pt-BR')
  logs.value += `[${timestamp}] ${mensagem}\n`
}

const conectarWebSocket = () => {
  try {
    adicionarLog('Conectando ao servidor...')
    connectionStatus.value = 'connecting'
    ws.value = new WebSocket('ws://localhost:5283/ws')
    
    ws.value.onopen = () => {
      connectionStatus.value = 'connected'
      adicionarLog('Conectado ao servidor')
    }
    
    ws.value.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data)
        executionReport.value = data
        adicionarLog('Execution Report recebido')
      } catch (e) {
        adicionarLog(`Servidor: ${JSON.stringify(event.data)}`)
      }
    }
    
    ws.value.onerror = (error) => {
      connectionStatus.value = 'failed'
      adicionarLog(`Erro na conexão: ${JSON.stringify(error)}`)
    }
    
    ws.value.onclose = () => {
      connectionStatus.value = 'failed'
      adicionarLog('Desconectado do servidor')
    }
  } catch (error) {
    connectionStatus.value = 'failed'
    adicionarLog(`Erro ao conectar: ${error.message}`)
  }
}

const desconectarWebSocket = () => {
  if (ws.value) {
    ws.value.close()
  }
}

const enviarOrdem = () => {
  submitted.value = true
  if (!form.value?.checkValidity()) return

  if (ws.value && ws.value.readyState === WebSocket.OPEN) {
    const payload = JSON.stringify({
      Sy: ativo.value,
      Si: tipo.value,
      Qt: quantidade.value,
      Pr: preco.value
    })
    ws.value.send(payload)
    adicionarLog('Ordem enviada ao servidor')
  } else {
    adicionarLog('Erro: Servidor desconectado')
  }

  resetarCampos()
}

function resetarCampos() {
  ativo.value = ''
  tipo.value = ''
  quantidade.value = ''
  preco.value = ''
  submitted.value = false
}

onMounted(() => {
  adicionarLog('-- não deixe de conferir os comments no código dos servidores :) --')
  conectarWebSocket()
})

onUnmounted(() => {
  desconectarWebSocket()
})
</script>

<template>
  <div class="page-container">
    <h1>Gerador de Ordens</h1>
    <div class="content-wrapper">
      <div class="form-section">
        <form ref="form" @submit.prevent="enviarOrdem" novalidate :class="{ submitted }">
          <button v-if="connectionStatus === 'failed'" @click.prevent="conectarWebSocket" class="reconnect-btn" title="Reconectar ao servidor">
            ⟲
          </button>
          <div class="form-group">
            <label for="ativo">Ativo:</label>
            <select id="ativo" v-model="ativo" required>
              <option value="">Selecione um ativo</option>
              <option value="PETR4">PETR4</option>
              <option value="VALE3">VALE3</option>
              <option value="VIIA4">VIIA4</option>
            </select>
          </div>

          <div class="form-group">
            <label for="tipo">Tipo de Ordem:</label>
            <select id="tipo" v-model="tipo" required>
              <option value="">Selecione o tipo</option>
              <option value="Compra">Compra</option>
              <option value="Venda">Venda</option>
            </select>
          </div>

          <div class="form-group">
            <label for="quantidade">Quantidade:</label>
            <input id="quantidade" v-model.number="quantidade" type="number" min="1" required />
          </div>

          <div class="form-group">
            <label for="preco">Preço:</label>
            <input id="preco" v-model.number="preco" type="number" step="0.01" min="0" required />
          </div>

          <button class="submit-button" type="submit" :disabled="connectionStatus === 'failed'">Enviar Ordem</button>
        </form>
      </div>
      
      <div class="right-section">
        <ExecutionReport :report="executionReport" />
      </div>
    </div>

    <div class="logs-section">
      <LogRegister :logs="logs" />
    </div>
  </div>
</template>

<style scoped>
.page-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 100vh;
  padding: 2rem;
}

h1 {
  font-weight: 500;
  font-size: 2.6rem;
  margin-bottom: 2rem;
  text-align: center;
}

.content-wrapper {
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 2rem;
}

.form-section,
.right-section,
.logs-section {
  display: flex;
  justify-content: center;
}

form {
  background: var(--color-background-soft);
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 520px;
  position: relative;
}

.reconnect-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 2rem;
  height: 2rem;
  padding: 0;
  background-color: #ef4444;
  border: none;
  border-radius: 50%;
  color: white;
  font-size: 1.2rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
}

.reconnect-btn:hover {
  background-color: #dc2626;
  transform: scale(1.1);
}

.reconnect-btn:active {
  transform: scale(0.95);
}

.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 1.5rem;
}

label {
  font-weight: 500;
  margin-bottom: 0.5rem;
  color: var(--color-text);
}

select,
input {
  padding: 0.75rem;
  border: 1px solid var(--color-border);
  border-radius: 4px;
  font-size: 1rem;
  background-color: var(--color-background);
  color: var(--color-text);
  transition: border-color 0.3s;
}

form.submitted select:invalid,
form.submitted input:invalid {
  border-color: #ef4444;
}

select:focus,
input:focus {
  outline: none;
  border-color: var(--color-border-hover);
}

.submit-button {
  width: 100%;
  padding: 0.75rem;
  background-color: var(--color-background-soft);
  border: 2px solid var(--color-border);
  border-radius: 4px;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s;
  color: var(--color-text);
}

.submit-button:hover {
  background-color: var(--color-border);
  border-color: var(--color-border);
  transform: scale(1.02);
}

.submit-button:active {
  transform: scale(0.98);
}

@media (min-width: 1024px) {
  .content-wrapper {
    flex-direction: row;
    align-items: flex-start;
    justify-content: center;
    max-width: 1200px;
    margin: 0 auto;
    gap: 2rem;
  }

  .form-section {
    flex: 1;
    width: 20rem;
    max-width: 520px;
    height: 41rem;
  }

  .right-section {
    flex: 1;
    max-width: 520px;
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  form {
    max-width: 520px;
    width: 100%;
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
  }
}
</style>
