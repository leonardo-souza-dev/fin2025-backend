import { Request, Response, Router } from 'express'
import Sqlitedb from '../infra/sqlitedb'
import Bank from '../models/bank'

export default class BankController {

    private router: Router
    private db: Sqlitedb

    private static tableName: string = 'banks'

    constructor() {
        this.router = Router()
        this.db = new Sqlitedb()
        this.initializeRoutes()
    }

    private initializeRoutes() {
        this.router.get('/', this.getAll.bind(this))
    }

    private async getAll(req: Request, res: Response) {
        try {
            const configs = await this.db.getAll<Bank>(BankController.tableName)
            res.json(configs)
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    public getRouter() {
        return this.router
    }
}
