import SimpleTransactionRepository from '../../infra/simple-transaction-repository'
import TransferRepository from '../../infra/transfer-repository'
import RecurrenceRepository from '../../infra/recurrence-repository'

import SimpleTransaction from '../../models/simple-transaction'
import Recurrence from '../../models/recurrence'

import Transaction from '../../models/transaction'
import Transfer from '../../models/transfer'

export default class SimpleTransactionService {

    private simpleTransactionRepository: SimpleTransactionRepository = new SimpleTransactionRepository()

    async getAllActive(): Promise<SimpleTransaction[]> {
        const simpleTransactions = await this.simpleTransactionRepository.getAll()        

        const simpleTransactionsActive = simpleTransactions.filter(simpleTransaction => simpleTransaction.isActive)
        return simpleTransactionsActive
    }

    async getByIdActive(id: number): Promise<SimpleTransaction> {
        if (id === undefined || id === null) {
            throw new Error('idType is required')
        }
            const simpleTransactionEntity = await this.simpleTransactionRepository.getById(id)
            if (!simpleTransactionEntity) {
                throw new Error(`SimpleTransaction with id ${id} not found`)
            }
            if (!simpleTransactionEntity.isActive) {
                throw new Error(`SimpleTransaction with id ${id} is not active`)
            }
            return simpleTransactionEntity
    }

    async upsert(transactionRequest: SimpleTransaction) : Promise<SimpleTransaction> {

        const isCreate = transactionRequest.id === undefined || transactionRequest.id === null

        if (isCreate) {
            var simpleTransactionCreating = new SimpleTransaction(
                transactionRequest.date, 
                transactionRequest.description, 
                transactionRequest.accountId, 
                transactionRequest.amount,
                transactionRequest.isActive,
                transactionRequest.isRecurrent,
                null
            )

            const idCreate = await this.simpleTransactionRepository.create(simpleTransactionCreating)

            return simpleTransactionCreating             
        } else {
            var simpleTransactionUpdating = new SimpleTransaction(
                transactionRequest.date, 
                transactionRequest.description, 
                transactionRequest.accountId, 
                transactionRequest.amount,
                transactionRequest.isActive,
                transactionRequest.isRecurrent,
                transactionRequest.id
            )
            
            await this.simpleTransactionRepository.update(simpleTransactionUpdating)

            return simpleTransactionUpdating
        }
    }

    async delete(id: number) : Promise<void> {
        const simpleTransaction = await this.simpleTransactionRepository.getById(id)
        if (!simpleTransaction) {
            throw new Error(`SimpleTransaction with id ${id} not found`)
        }
        simpleTransaction.isActive = false
        await this.simpleTransactionRepository.update(simpleTransaction)
    }
}