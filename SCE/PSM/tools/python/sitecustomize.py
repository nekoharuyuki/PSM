
import sys
import os

prefix, tmp = os.path.split( sys.executable )

sys.path = [ 
    prefix + "/python25.zip",
    prefix + "/DLLs",
    prefix + "/pil.zip",
    ]
