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
        // postman ok
        this.router.get('/', this.getAll.bind(this));

        // already migrated to dotnet core
        // postman ok
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
                console.log('Creating new config...');
                await this.db.create<Config>(ConfigController.tableName, config);
                res.status(201).json(config);
                return;
            }
            console.log('Updating existing config...');

            const existingConfigs = await this.db.getAll<Config>(ConfigController.tableName);
            const existingConfig = existingConfigs.find(c => c.id === config.id);
            if (!existingConfig) {
                console.log(`Config with id ${config.id} not found`);
                res.status(404).json({ error: `Config not found with id ${config.id}` });
                return;
            }
            if (!existingConfig.isActive) {
                console.log(`Config with id ${config.id} is not active`);
                res.status(400).json({ error: `Config with id ${config.id} is not active` });
                return;            }

            if (config.key !== existingConfig.key) {
                console.log(`Config with id ${config.id} doesn't have key ${config.key}`);
                res.status(400).json({ error: `Config with id ${config.id} doesn't have key ${config.key}` });
                return;
            }
            console.log(`Updating config with id ${config.id}...`);
            console.log(config)
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
