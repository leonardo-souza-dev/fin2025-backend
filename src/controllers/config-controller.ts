import { Request, Response, Router } from 'express';
import Sqlitedb from '../infra/sqlitedb'
import Config from '../models/config'

export default class ConfigController {

    private router: Router;
    private db: Sqlitedb;

    private static tableName: string = 'configs'

    constructor() {
        this.router = Router();
        this.db = new Sqlitedb();
        this.initializeRoutes();
    }

    private initializeRoutes() {
        this.router.get('/', this.getAll.bind(this));
        this.router.get('/:id', this.getById.bind(this));
        this.router.post('/', this.create.bind(this));
        this.router.put('/:id', this.update.bind(this));
        this.router.delete('/:id', this.delete.bind(this));
    }

    private async getAll(req: Request, res: Response) {
        try {
            const configs = await this.db.getAll<Config>(ConfigController.tableName);
            res.json(configs);
        } catch (err: any) {
            res.status(500).json({ error: err.message });
        }
    }

    private async getById(req: Request, res: Response) {
        try {
            const id: number = parseInt(req.params.id, 10);
            const config = await this.db.getById<Config>(ConfigController.tableName, id);
            res.json(config);
        } catch (err: any) {
            res.status(500).json({ error: err.message });
        }
    }

    private async create(req: Request, res: Response) {
        try {
            const body = req.body
            const config = new Config(body.key, body.value, body.isActive, null)
            const id = await this.db.create<Config>(ConfigController.tableName, config)
            res.json(id)
        } catch (err: any) {
            res.status(500).json({ error: err.message });
        }
    }

    private async update(req: Request, res: Response) {
        try {
            const config = req.body;
            config.id = parseInt(req.params.id, 10);
            await this.db.update<Config>(ConfigController.tableName, config);
            res.json(config);
        } catch (err: any) {
            res.status(500).json({ error: err.message });
        }
    }

    private async delete(req: Request, res: Response) {
        try {
            const id = parseInt(req.params.id, 10);
            await this.db.delete(ConfigController.tableName, id);
            res.json(id);
        } catch (err: any) {
            res.status(500).json({ error: err.message });
        }
    }

    public getRouter() {
        return this.router
    }
}
