import SimpleTransactionRepository from '../../infra/simple-transaction-repository'
import TransferRepository from '../../infra/transfer-repository'
import RecurrenceRepository from '../../infra/recurrence-repository'

import SimpleTransaction from '../../models/simple-transaction'
import Recurrence from '../../models/recurrence'

import Transaction from '../../models/transaction'
import Transfer from '../../models/transfer'
import SimpleTransactionService from './simple-transaction-service'

export default class TransactionService {

    private simpleTransactionService: SimpleTransactionService = new SimpleTransactionService()
    private transferRepository: TransferRepository = new TransferRepository()
    private recurrenceRepository: RecurrenceRepository = new RecurrenceRepository()

    async getAll(): Promise<Transaction[]> {
        
        const simpleTransactionsActive = await this.simpleTransactionService.getAllActive()
        const transfers = await this.transferRepository.getAll()
        //const recurrences = await this.recurrenceRepository.getAllActive()

        let transactions: Transaction[] = []

        simpleTransactionsActive.forEach(simpleTransaction => {
                transactions.push(new Transaction(
                    simpleTransaction.date,
                    simpleTransaction.description,
                    simpleTransaction.accountId,
                    simpleTransaction.amount,
                    'SIMPLE',
                    null,
                    true,
                    false,
                    simpleTransaction.id
                ))
        })

        transfers.forEach(transfer => {
            if (transfer.isActive) {
                transactions.push(new Transaction(
                    transfer.date,
                    transfer.description,
                    transfer.sourceAccountId,
                    transfer.amount,
                    'TRANSFER',
                    transfer.destinationAccountId,
                    true,
                    false,
                    transfer.id
                ))
                transactions.push(new Transaction(
                    transfer.date,
                    transfer.description,
                    transfer.destinationAccountId,
                    transfer.amount * -1,
                    'TRANSFER',
                    transfer.sourceAccountId,
                    true,
                    false,
                    transfer.id
                ))
            }
        })

        /*
        recurrences.forEach(recurrence => {
            const startYear = recurrence.startYearMonth.split('-')[0]
            const startMonth = recurrence.startYearMonth.split('-')[1]

            const endYear = recurrence.endYearMonth.split('-')[0]
            const endMonth = recurrence.endYearMonth.split('-')[1]

            const deltaMonths = ((parseInt(endYear) - parseInt(startYear)) * 12 + parseInt(endMonth) - parseInt(startMonth)) + 1

            for (let i = 0; i < deltaMonths; i++) {
                let year = parseInt(startYear)
                let month = parseInt(startMonth) + i
                if (month > 12) {
                    year++
                    month -= 12
                }

                const date = `${year}-${month.toString().padStart(2,"0")}-${recurrence.day}`

                transactions.push(new Transaction(
                    date,
                    recurrence.name,
                    recurrence.accountId,
                    recurrence.amount,
                    'RECURRENCE',
                    null,
                    true,
                    true
                ))
            }
        })
        */

        return transactions        
    }

    async getById(idType: string): Promise<Transaction | null> {
        if (idType === undefined || idType === null) {
            throw new Error('idType is required')
        }
        const { type, id } = this.getIdType(idType)
        
        if (type === 'TRANSFER') {
            const transferEntity = await this.transferRepository.getById(id)
            if (transferEntity) {
                return new Transaction(
                    transferEntity.date,
                    transferEntity.description,
                    transferEntity.sourceAccountId,
                    transferEntity.amount,
                    'TRANSFER',
                    transferEntity.destinationAccountId,
                    true,
                    false,
                    transferEntity.id
                )
            }
        } 
        
        if (type === 'SIMPLE') {
            const simpleTransactionEntity = await this.simpleTransactionService.getByIdActive(id)
            return new Transaction(
                simpleTransactionEntity.date,
                simpleTransactionEntity.description,
                simpleTransactionEntity.accountId,
                simpleTransactionEntity.amount,
                'SIMPLE',
                null,
                true,
                false,
                simpleTransactionEntity.id
            )
        }

        throw new Error(`Transaction with id ${id} not found`)
    }

    async upsert(transactionRequest: any) : Promise<Transaction> {
        
        if (transactionRequest === undefined || transactionRequest === null) {
            throw new Error('transactionRequest is required')
        }

        const isCreate = transactionRequest.id === undefined || transactionRequest.id === null
        const type = transactionRequest.otherAccountId === null ? 'SIMPLE' : 'TRANSFER'

        if (type === 'SIMPLE') {
            const simpleTransactionUpserted = await this.simpleTransactionService.upsert(
                new SimpleTransaction(
                    transactionRequest.date, 
                    transactionRequest.description, 
                    parseInt(transactionRequest.refAccountId), 
                    transactionRequest.amount,
                    transactionRequest.isActive,
                    transactionRequest.isRecurrent,
                    transactionRequest.id
                )
            )

            return new Transaction(
                transactionRequest.date,
                transactionRequest.description,
                parseInt(transactionRequest.refAccountId),
                transactionRequest.amount,
                'SIMPLE',
                parseInt(transactionRequest.otherAccountId),
                transactionRequest.isActive,
                transactionRequest.isRecurrent,
                simpleTransactionUpserted.id
            )
        }

        if (isCreate && type === 'TRANSFER') {
            const idCreate = await this.transferRepository.create(
                new Transfer(
                    transactionRequest.date, 
                    transactionRequest.description, 
                    parseInt(transactionRequest.refAccountId), 
                    transactionRequest.amount,
                    transactionRequest.isActive,
                    transactionRequest.isRecurrent,
                    parseInt(transactionRequest.otherAccountId),
                    null
                )
            )

            return new Transaction(
                transactionRequest.date,
                transactionRequest.description,
                parseInt(transactionRequest.refAccountId),
                transactionRequest.amount,
                'TRANSFER',
                parseInt(transactionRequest.otherAccountId),
                transactionRequest.isActive,
                transactionRequest.isRecurrent,
                idCreate
            )
        } else if (!isCreate && type === 'TRANSFER') {
            await this.transferRepository.update(
                new Transfer(
                    transactionRequest.date, 
                    transactionRequest.description, 
                    parseInt(transactionRequest.refAccountId), 
                    transactionRequest.amount,
                    transactionRequest.isActive,
                    transactionRequest.isRecurrent,
                    parseInt(transactionRequest.otherAccountId),
                    parseInt(transactionRequest.id)
                )
            )

            return new Transaction(
                transactionRequest.date,
                transactionRequest.description,
                parseInt(transactionRequest.refAccountId),
                transactionRequest.amount,
                'TRANSFER',
                parseInt(transactionRequest.otherAccountId),
                transactionRequest.isActive,
                transactionRequest.isRecurrent,
                parseInt(transactionRequest.id)
            )
        }

        throw new Error('Error on upsert transaction')
    }

    async delete(idType: string) : Promise<void> {
        if (idType === undefined || idType === null) {
            throw new Error('idType is required')
        }

        if (this.getIdType(idType.toString()).type === 'SIMPLE') {
            await this.simpleTransactionService.delete(this.getIdType(idType.toString()).id)
        } else if (this.getIdType(idType.toString()).type === 'TRANSFER') {
            const transfer = await this.transferRepository.getById(this.getIdType(idType.toString()).id)
            if (transfer) {
                transfer.isActive = false
                await this.transferRepository.update(transfer)
            } else {
                throw new Error(`Transaction with idType ${idType} not found`)
            }
        } else  {
            throw new Error(`Transaction with idType ${idType} not found`)
        } 
    }

    private getIdType(idType: string): {id: number, type: string} {
        let id: number = 0
        let type: string = ''

        if (idType.includes('_')) { 
            id = parseInt(idType.split('_')[0], 10)
            type = idType.split('_')[1]
        } else {
            id = parseInt(idType, 10)
        }

        return {id, type}
    }
}