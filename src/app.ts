import dotenv from 'dotenv'
import cors from 'cors'
import express from 'express'
import cookieParser from 'cookie-parser'

import AccountController from './controllers/account-controller'
import TransactionController from './controllers/transaction-controller'
import ConfigController from './controllers/config-controller'
import BankController from './controllers/bank-controller'
import AuthController from './controllers/auth-controller'
import SimpleTransactionService from './services/simple-transaction-service'
import TransactionService from './services/transaction-service'
import TransferRepository from './infra/transfer-repository'
import RecurrenceService from './services/recurrence-service'

dotenv.config()

const app = express()

const corsOptions = {
    'origin': process.env.CORS_ORIGIN,
    "methods": "GET,HEAD,PUT,PATCH,POST,DELETE",
    "preflightContinue": true,
    "exposedHeaders": 'Content-Range,X-Content-Range',
    "credentials": true
}
app.use(cors(corsOptions))
app.use(express.json())
app.use(cookieParser())

const simplesTransactionService = new SimpleTransactionService()
const transferRepository = new TransferRepository()
const recurrenceService = new RecurrenceService()

const transactionService = new TransactionService(
    simplesTransactionService, 
    transferRepository,
    recurrenceService
)

const accountController = new AccountController()
const transactionController = new TransactionController(transactionService)
const configController = new ConfigController()
const bankController = new BankController()
const authController = new AuthController()

app.use('/api/accounts', accountController.getRouter())
app.use('/api/transactions', transactionController.getRouter())
app.use('/api/configs', configController.getRouter())
app.use('/api/banks', bankController.getRouter())
app.use("/api/auth", authController.getRouter())

export default app