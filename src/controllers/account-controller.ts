import { Request, Response, Router } from 'express'
import Sqlitedb from '../infra/sqlitedb'
import Account from '../models/account'
import { authenticateToken } from '../middleware/auth.middleware'

export default class AccountController {

    private router: Router
    private db: Sqlitedb

    constructor() {
        this.router = Router()
        this.db = new Sqlitedb()
        this.initializeRoutes()
    }

    private initializeRoutes() {
        // already migrated to dotnet core
        // postman ok2
        this.router.get('/', authenticateToken, this.getAllAccounts.bind(this))
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


    public getRouter() {
        return this.router
    }
}
