from flask import Flask, json, jsonify, request
from flask_restful import Api, Resource
from flask_cors import CORS, cross_origin
from flask_sqlalchemy import SQLAlchemy
import os

app = Flask(__name__, static_folder='files', static_url_path='/files')
cors = CORS(app)
app.config['CORS_HEADER'] = 'Content-Type'
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///db.sqlite3'
app.config['UPLOAD_FOLDER'] = "files/"
api = Api(app)
db = SQLAlchemy(app)

class Files(db.Model):
    file_id = db.Column('file_id', db.Integer, primary_key = True)
    file_name = db.Column(db.String(30))
    file_desc = db.Column(db.String(200))  
    file_path = db.Column(db.String(200))
    def __init__(self, file_name, file_desc):
        self.file_name = file_name
        self.file_desc = file_desc
    @property
    def json(self):
        return {'file_id':self.file_id,'file_name':self.file_name,'category':self.file_desc,'file_path':""}

class FileListAPI(Resource):
    def get(self):
        resp, resp_arr = {}, []
        for i in Files.query.all():
            if i.file_desc not in resp:
                resp[i.file_desc] = []
            resp[i.file_desc].append(i.json)
        for i in resp:
            resp_arr.append({'category':i, 'data':resp[i]})
        return jsonify(resp_arr)
    def post(self):
        file_name,file_desc,uploaded_file = request.form['file_name'],request.form['file_desc'],request.files['uploaded_file']
        file_obj = Files(file_name, file_desc)
        db.session.add(file_obj)
        db.session.flush()
        file_obj.file_path = str(file_obj.file_id) + "_" + uploaded_file.filename
        uploaded_file.save(os.path.join(app.config['UPLOAD_FOLDER'], file_obj.file_path))
        db.session.commit()
        return jsonify({"message": "Saved successfully", "data": file_obj.json})

class FileAPI(Resource):
    def put(self, file_id):
        file_name,file_desc = request.form['file_name'],request.form['file_desc']
        file_obj = Files.query.get(file_id)
        if file_name:
            file_obj.file_name = file_name
        file_obj.file_desc = file_desc
        if len(request.files):
            os.remove(os.path.join(app.config['UPLOAD_FOLDER'], file_obj.file_path))
            file_obj.file_path = str(file_id) + "_" + request.files['uploaded_file'].filename
            request.files['uploaded_file'].save(os.path.join(app.config['UPLOAD_FOLDER'], file_obj.file_path))
        db.session.commit()
        return jsonify({"message": "Updated successfully"})
    def delete(self, file_id):
        Files.query.filter_by(file_id=file_id).delete()
        db.session.commit()
        return {"message": "Deleted successfully"}

db.create_all()

api.add_resource(FileAPI, '/file/<int:file_id>', endpoint = 'file')
api.add_resource(FileListAPI, '/files', endpoint = 'files')

if __name__ == "__main__":
    if len(Files.query.all()) < 1:
        file_obj = Files("SampleTool", "DUV Scanner")
        db.session.add(file_obj)
        db.session.commit()
        file_obj = Files("ClipTool", "GENERIC")
        db.session.add(file_obj)
        db.session.commit()
        file_obj = Files("Ditsmaker", "GENERIC")
        db.session.add(file_obj)
        db.session.commit()
        file_obj = Files("Lot Report Stripper", "GENERIC")
        db.session.add(file_obj)
        db.session.commit()
        file_obj = Files("TPMS", "GENERIC")
        db.session.add(file_obj)
        db.session.commit()

    app.run(host="0.0.0.0", debug=True)
