import { Request, Response, Router } from 'express'
import TransactionService from '../services/transaction-service'
import { authenticateToken } from '../middleware/auth.middleware'

export default class TransactionController {

    private transactionService: TransactionService
    private router = Router()

    constructor(transactionService: TransactionService) {
        this.transactionService = transactionService
        this.initializeRoutes()
    }

    private initializeRoutes() {
        // already migrated to dotnet core
        this.router.get('/', authenticateToken, this.getAll.bind(this))

        // already migrated to dotnet core
        this.router.put('/', authenticateToken, this.upsert.bind(this))

        // already migrated to dotnet core
        this.router.delete('/:idType', authenticateToken, this.delete.bind(this))
    }

    private async getAll(request: Request, response: Response) {
        try {
            const monthYear = request.query.monthYear as string
            const entities = await this.transactionService.getAllActive(monthYear)
            response.header('qtd', entities.length.toString())
            response.json(entities)
        } catch (err: any) {
            response.status(500).json({ error: err.message })
        }
    }

    private async upsert(request: Request, res: Response) : Promise<void> {
        
        try {
            const entity = await this.transactionService.upsert(request.body)
            res.json(entity)
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    private async delete(req: Request, res: Response) {
        try {
            await this.transactionService.delete(req.params.idType)
            res.status(204).send()
        } catch (err: any) {
            res.status(500).json({ error: err.message });
        }
    }

    public getRouter() {
        return this.router
    }
}
