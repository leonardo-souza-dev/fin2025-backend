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
        // already migrated to dotnet core
        this.router.get('/', this.getAll.bind(this));

        // already migrated to dotnet core
        this.router.put('/', this.upsert.bind(this));
    }

    private async getAll(req: Request, res: Response) {
        try {
            const configs = await this.db.getAll<Config>(ConfigController.tableName);
            res.json(configs);
        } catch (err: any) {
            res.status(500).json({ error: err.message });
        }
    }
    private async upsert(req: Request, res: Response) {
        try {
            const config = req.body;

            if (config.id === undefined) {
                config.isActive = true;
                await this.db.create<Config>(ConfigController.tableName, config);
                res.status(201).json(config);
                return;
            }

            const existingConfigs = await this.db.getAll<Config>(ConfigController.tableName);

            const existingConfig = existingConfigs.find(c => c.id === config.id);
            if (!existingConfig) {
                res.status(404).json({ error: `Config not found with id ${config.id}` });
                return;
            }
            if (!existingConfig.isActive) {
                res.status(400).json({ error: `Config with id ${config.id} is not active` });
                return;            }

            if (config.key !== existingConfig.key) {
                res.status(400).json({ error: `Config with id ${config.id} doesn't have key ${config.key}` });
                return;
            }
            await this.db.update<Config>(ConfigController.tableName, config);
            res.json(config);
        } catch (err: any) {
            res.status(500).json({ error: err.message });
        }
    }

    public getRouter() {
        return this.router
    }
}
