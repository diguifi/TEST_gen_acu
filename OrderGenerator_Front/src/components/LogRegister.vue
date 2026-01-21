<script setup>
defineProps({
  logs: {
    type: String,
    required: true
  }
})
</script>

<template>
  <div class="logs-container">
    <textarea id="logs" :value="logs" readonly></textarea>
  </div>
</template>

<style scoped>
.logs-container {
  margin-top: 2rem;
  display: flex;
  flex-direction: column;
  width: 55rem;
}

.logs-container label {
  font-weight: 500;
  margin-bottom: 0.5rem;
  color: var(--color-text);
}

textarea {
  padding: 0.75rem;
  border: 1px solid var(--color-border);
  border-radius: 4px;
  font-size: 0.875rem;
  background-color: var(--color-background);
  color: var(--color-text);
  font-family: monospace;
  height: 100%;
  min-height: 240px;
  resize: none;
  box-sizing: border-box;
}

@media (max-width: 1024px) {
  .logs-container {
    width: 27rem;
  }
}
</style>
