<script setup>
const props = defineProps({
  report: {
    type: Object,
    required: true
  }
})

const fields = [
  { short: 'CId', long: 'ClOrdId' },
  { short: 'OId', long: 'OrderId' },
  { short: 'EId', long: 'ExecId' },
  { short: 'Sy', long: 'Symbol' },
  { short: 'Si', long: 'Side' },
  { short: 'Sts', long: 'OrdStatus' },
  { short: 'ExTy', long: 'ExecType' },
  { short: 'Qt', long: 'OrderQty' },
  { short: 'CQt', long: 'CumQty' },
  { short: 'LQt', long: 'LeavesQty' },
  { short: 'LPx', long: 'LastPx' },
  { short: 'AvPx', long: 'AvgPx' },
  { short: 'Txt', long: 'Erro' }
]

const getFieldValue = (shortKey) => {
  return props.report && props.report[shortKey] !== undefined ? props.report[shortKey] : ''
}
</script>

<template>
  <div class="execution-report">
    <h3>Execution Report</h3>
    <div class="report-grid">
      <div v-for="field in fields" :key="field.short" class="report-field">
        <div v-if="field.short === 'Txt' && getFieldValue(field.short) === ''">
        </div>
        <div v-else>
          <label :for="field.short">{{ field.long }}:</label>
          <input :id="field.short" type="text" :value="getFieldValue(field.short)" readonly />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.execution-report {
  width: 100%;
  max-width: 520px;
  padding: 2rem;
  background: var(--color-background-soft);
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  height: 41rem;
}

h3 {
  font-weight: 500;
  font-size: 1.2rem;
  margin-bottom: 1.5rem;
  text-align: center;
  color: var(--color-text);
}

.report-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.report-field {
  display: flex;
  flex-direction: column;
}

label {
  font-weight: 500;
  margin-bottom: 0.5rem;
  color: var(--color-text);
  font-size: 0.875rem;
}

input {
  padding: 0.5rem;
  border: 1px solid var(--color-border);
  border-radius: 4px;
  font-size: 0.875rem;
  background-color: var(--color-background);
  color: var(--color-text);
  font-family: monospace;
}

@media (min-width: 1024px) {
  .execution-report {
    max-width: 520px;
  }

  .report-grid {
    grid-template-columns: 1fr 1fr;
  }
}
</style>
