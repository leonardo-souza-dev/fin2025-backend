import TransferRepository from '../infra/transfer-repository'
import RecurrenceService from '../services/recurrence-service'
import SimpleTransaction from '../models/simple-transaction'
import Transaction from '../models/transaction'
import Transfer from '../models/transfer'
import SimpleTransactionService from './simple-transaction-service'

export default class TransactionService {

    private simpleTransactionService: SimpleTransactionService
    private transferRepository: TransferRepository
    private recurrenceService: RecurrenceService

    constructor(
        simpleTransactionService: SimpleTransactionService, 
        transferRepository: TransferRepository, 
        recurrenceService: RecurrenceService
    ) {
        this.simpleTransactionService = simpleTransactionService
        this.transferRepository = transferRepository
        this.recurrenceService = recurrenceService
    }

    async getAllActive(monthSlashYear: string): Promise<Transaction[]> {

        let transactions: Transaction[] = []

        const simpleTransactionsActive = await this.simpleTransactionService.getAllActive()
        
        simpleTransactionsActive.forEach(simpleTransactionActive => {
            transactions.push(new Transaction(
                simpleTransactionActive.date,//
                simpleTransactionActive.description,//
                simpleTransactionActive.accountId,//
                simpleTransactionActive.amount,//
                'SIMPLE',
                null,
                simpleTransactionActive.isActive,
                false,
                simpleTransactionActive.id//
            ))
        })



        const transfers = await this.transferRepository.getAll()
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



        const monthSlashYearSplit = monthSlashYear.split("/")

        const year = monthSlashYearSplit[1]
        const month = monthSlashYearSplit[0].padStart(2, '0')

        const monthYear = `${year}-${month}`

        const recurrences = await this.recurrenceService.getAllActive(monthYear)

        if (recurrences !== null && recurrences !== undefined) {
            const activeRecurrencesMonthYear = recurrences.filter(
                r => r.isActive &&
                r.startYearMonth <= monthYear &&
                r.endYearMonth >= monthYear
            ).map(r => 
                    new Transaction(
                        `${year}-${month}-${r.day}`,
                        r.name,
                        r.accountId,
                        r.amount,
                        'RECURRENCE',
                        null,
                        true,
                        true,
                        null
                    )            
            )
            transactions = transactions.concat(activeRecurrencesMonthYear)
        }

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

        if (this.getIdType(idType).type === 'SIMPLE') {
            await this.simpleTransactionService.delete(this.getIdType(idType).id)
        } else if (this.getIdType(idType).type === 'TRANSFER') {
            const transfer = await this.transferRepository.getById(this.getIdType(idType).id)
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