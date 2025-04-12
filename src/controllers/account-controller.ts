import { Request, Response, Router } from 'express'
import Sqlitedb from '../infra/sqlitedb'
import Account from '../models/account'

export default class AccountController {

    private router: Router
    private db: Sqlitedb

    constructor() {
        this.router = Router()
        this.db = new Sqlitedb()
        this.initializeRoutes()
    }

    private initializeRoutes() {
        this.router.get('/', this.getAllAccounts.bind(this))
        this.router.get('/:id', this.getAccountById.bind(this))
        this.router.post('/', this.createAccount.bind(this))
        this.router.put('/:id', this.updateAccount.bind(this))
        this.router.delete('/:id', this.deleteAccount.bind(this))
    }

    private async getAllAccounts(req: Request, res: Response) {
        try {
            const accounts = await this.db.getAll<Account>('accounts')
            const activeAccounts = accounts.filter(account => account.isActive)
            res.json(activeAccounts)
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    private async getAccountById(req: Request, res: Response) {
        try {
            const id: number = parseInt(req.params.id, 10)
            const account = await this.db.getById<Account>('accounts', id)
            res.json(account)
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    private async createAccount(req: Request, res: Response) {
        try {
            const body = req.body
            const account = new Account(body.name, body.isActive, body.comments, null)
            const id = await this.db.create<Account>('accounts', account)
            res.json(id)
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    private async updateAccount(req: Request, res: Response) {
        try {
            const account = req.body
            account.id = parseInt(req.params.id, 10)
            await this.db.update<Account>('accounts', account)
            res.json(account)
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    private async deleteAccount(req: Request, res: Response) {
        try {
            const id = parseInt(req.params.id, 10)
            await this.db.delete('accounts', id)
            res.json(id)
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    public getRouter() {
        return this.router
    }
}
